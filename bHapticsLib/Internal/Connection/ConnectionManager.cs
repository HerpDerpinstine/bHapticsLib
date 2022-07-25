﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using bHapticsLib.Internal.Connection.Models;
using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal.Connection
{
    internal class ConnectionManager : ThreadedTask
    {
        #region Type Cache
        private static readonly Type intType = typeof(int);
        private static readonly Type byteType = typeof(byte);
        private static readonly Type dotPointType = typeof(DotPoint);
        #endregion

        #region Queue
        private List<RegisterRequest> RegisterCache = new List<RegisterRequest>();
        private ThreadSafeQueue<RegisterRequest> RegisterQueue = new ThreadSafeQueue<RegisterRequest>();
        private ThreadSafeQueue<SubmitRequest> SubmitQueue = new ThreadSafeQueue<SubmitRequest>();
        #endregion

        #region Threading
        internal string ID, Name;
        internal bool TryToReconnect;
        internal int MaxRetries;
        internal WebSocketConnection Socket;
        private PlayerPacket Packet = new PlayerPacket();
        private bool ShouldRun = true;

        internal override bool BeginInitInternal()
        {
            if (Socket != null)
                EndInit();

            Socket = new WebSocketConnection(this, ID, Name, TryToReconnect, MaxRetries);
            ShouldRun = true;
            return true;
        }

        internal override bool EndInitInternal()
        {
            if (Socket == null)
                return false;

            ShouldRun = false;
            while (IsAlive()) { Thread.Sleep(1); }

            RegisterCache.Clear();
            Socket.Dispose();
            Socket = null;

            return true;
        }

        internal override void WithinThread()
        {
            while (ShouldRun)
            {
                if (Socket.FirstTry)
                {
                    Socket.FirstTry = false;
                    Socket.TryConnect();
                }

                if (IsConnected())
                {
                    RegisterRequest registerRequest;
                    while ((registerRequest = RegisterQueue.Dequeue()) != null)
                        Packet.Register.Add(registerRequest);

                    SubmitRequest submitRequest;
                    while ((submitRequest = SubmitQueue.Dequeue()) != null)
                        Packet.Submit.Add(submitRequest);

                    Socket.Send(Packet);
                    Packet.Clear();
                }

                if (ShouldRun)
                    Thread.Sleep(1);
            }
        }

        internal void QueueRegisterCache()
        {
            int cacheCount = RegisterCache.Count;
            if (cacheCount <= 0)
                return;
            for (int i = 0; i < cacheCount; i++)
            {
                RegisterRequest request = RegisterCache[i];
                if ((request == null)
                    || request.IsNull)
                    continue;
                RegisterQueue.Enqueue(request);
            }
        }
        internal bool IsConnected() => Socket?.IsConnected() ?? false;
        #endregion

        #region Device
        internal int GetConnectedDeviceCount() => Socket?.LastResponse?.ConnectedDeviceCount ?? 0;
        internal bool IsDeviceConnected(PositionType type)
        {
            if ((type == PositionType.VestFront)
                || (type == PositionType.VestBack))
                type = PositionType.Vest;
            return Socket?.LastResponse?.ConnectedPositions?.ContainsValue(type) ?? false;
        }
        internal int[] GetDeviceStatus(PositionType type)
        {
            if ((Socket == null)
                || (Socket.LastResponse == null))
                return default;

            JSONNode statusArray = Socket.LastResponse.Status;
            if (type == PositionType.Vest)
            {
                JSONNode frontStatus = statusArray[PositionType.VestFront.ToString()];
                JSONNode backStatus = statusArray[PositionType.VestBack.ToString()];

                int totalCount = frontStatus.Count + backStatus.Count;
                int[] returnval = new int[totalCount];
                for (int i = 0; i < totalCount; i++)
                {
                    if (i < frontStatus.Count)
                        returnval[i] = frontStatus[i].AsInt;
                    else
                        returnval[i] = backStatus[i - frontStatus.Count].AsInt;
                }

                return returnval;
            }
            else
            {
                JSONNode posArray = statusArray[type.ToString()];
                int totalCount = posArray.Count;
                int[] returnval = new int[totalCount];
                for (int i = 0; i < totalCount; i++)
                    returnval[i] = posArray[i].AsInt;
                return returnval;
            }
        }
        #endregion

        #region IsPlaying
        internal bool IsPlaying(string key) => Socket?.LastResponse?.ActiveKeys?.ContainsValue(key) ?? false;
        internal bool IsPlayingAny() => (Socket?.LastResponse?.ActiveKeys?.Count > 0);
        #endregion

        #region StopPlaying
        internal void StopPlaying(string key)
        {
            if (!IsAlive() || !IsConnected())
                return;
            SubmitQueue.Enqueue(new SubmitRequest { key = key, type = "turnOff" });
        }
        internal void StopPlayingAll()
        {
            if (!IsAlive() || !IsConnected())
                return;
            SubmitQueue.Enqueue(new SubmitRequest { type = "turnOffAll" });
        }
        #endregion

        #region PatternRegister
        internal bool IsPatternRegistered(string key) => Socket?.LastResponse?.RegisteredKeys?.ContainsValue(key) ?? false;

        internal void RegisterPatternFromFile(string key, string tactFilePath)
        {
            if (!File.Exists(tactFilePath))
                return; // To-Do: Exception Here

            RegisterPatternFromJson(key, File.ReadAllText(tactFilePath));
        }

        internal void RegisterPatternFromJson(string key, string tactFileStr)
        {
            if (string.IsNullOrEmpty(key))
                return; // To-Do: Exception Here

            if (string.IsNullOrEmpty(tactFileStr))
                return; // To-Do: Exception Here

            RegisterRequest request = new RegisterRequest();
            request.key = key;
            request.project = JSON.Parse(tactFileStr)["project"].AsObject;

            RegisterCache.Add(request);
            RegisterQueue.Enqueue(request);
        }
        #endregion

        #region Submit
        internal void Submit<A, B>(
            string key,
            int durationMillis,
            PositionType position,
            A dotPoints,
            B pathPoints,
            MirrorDirection dotMirrorDirection = MirrorDirection.None)
            where A : IList, ICollection
            where B : IList<PathPoint>, ICollection<PathPoint>
        {
            if (!IsAlive() || !IsConnected() || !IsDeviceConnected(position))
                return;

            if (position == PositionType.Vest)
            {
                Submit($"{key}Front", durationMillis, PositionType.VestFront, dotPoints, pathPoints, dotMirrorDirection);
                Submit($"{key}Back", durationMillis, PositionType.VestBack, dotPoints, pathPoints, dotMirrorDirection);
                return;
            }

            SubmitRequest request = new SubmitRequest { key = key, type = "frame" };
            request.Frame.durationMillis = durationMillis;
            request.Frame.position = position;

            if ((dotPoints != null) && (dotPoints.Count > 0))
            {
                switch (dotMirrorDirection)
                {
                    case MirrorDirection.Horizontal:
                        MirrorHorizontal(ref dotPoints, position);
                        goto default;

                    case MirrorDirection.Vertical:
                        MirrorVertical(ref dotPoints, position);
                        goto default;

                    case MirrorDirection.Both:
                        MirrorHorizontal(ref dotPoints, position);
                        MirrorVertical(ref dotPoints, position);
                        goto default;

                    default:
                        break;
                }

                Type pointType = null;
                for (int i = 0; (i < dotPoints.Count); i++)
                {
                    object point = dotPoints[i];
                    if (point == null)
                        continue;

                    if (pointType == null)
                        pointType = point.GetType();

                    if ((pointType == intType) || (pointType == byteType))
                    {
                        JSONObject node = new JSONObject();
                        node["index"] = i.Clamp(0, bHapticsManager.MaxMotorsPerDotPoint);
                        node["intensity"] = ((int)point).Clamp(0, bHapticsManager.MaxIntensity);
                        request.Frame.dotPoints.Add(node);
                    }
                    else if (pointType == dotPointType)
                        request.Frame.dotPoints.Add((point as DotPoint).node);
                }
            }

            SubmitQueue.Enqueue(request);
        }
        #endregion

        #region SubmitRegistered
        internal void SubmitRegistered(string key, string altKey = null, ScaleOption scaleOption = null, RotationOption rotationOption = null)
        {
            if (!IsAlive() || !IsConnected())
                return;

            SubmitRequest request = new SubmitRequest { key = key, type = "key" };

            if (!string.IsNullOrEmpty(altKey))
                request.Parameters["altKey"] = altKey;

            if (scaleOption != null)
                request.Parameters["scaleOption"] = scaleOption.node;

            if (rotationOption != null)
                request.Parameters["rotationOption"] = rotationOption.node;

            SubmitQueue.Enqueue(request);
        }
        #endregion

        #region MirrorHorizontal
        private static void MirrorHorizontal<A>(ref A dotPoints, PositionType position) where A : IList, ICollection
        {
            switch (position)
            {
                case PositionType.Head:
                    dotPoints.Reverse(0, 6);
                    break;

                case PositionType.VestFront:
                case PositionType.VestBack:
                    for (int i = 0; (i + 3) < dotPoints.Count; i += 4)
                        dotPoints.Reverse(i, 4);
                    break;

                case PositionType.ForearmL:
                case PositionType.ForearmR:
                    // TO-DO
                    break;

                case PositionType.GloveL:
                case PositionType.GloveR:
                    // TO-DO
                    break;

                case PositionType.FootL:
                case PositionType.FootR:
                    // TO-DO
                    break;

                default:
                    break;
            }
        }
        private static void MirrorVertical<A>(ref A dotPoints, PositionType position) where A : IList, ICollection
        {
            switch (position)
            {
                case PositionType.VestFront:
                case PositionType.VestBack:
                    // TO-DO
                    break;

                case PositionType.ForearmL:
                case PositionType.ForearmR:
                    // TO-DO
                    break;

                case PositionType.GloveL:
                case PositionType.GloveR:
                    // TO-DO
                    break;

                case PositionType.HandL:
                case PositionType.HandR:
                    dotPoints.Reverse(0, 3);
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
