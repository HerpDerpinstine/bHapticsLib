﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using bHapticsLib.Internal.Connection.Models;
using bHapticsLib.SimpleJSON;

namespace bHapticsLib.Internal.Connection
{
    internal class ConnectionManager : ThreadedTask
    {
        private bool ShouldRun = true;

        internal string ID, Name;
        internal bool TryReconnect;
        internal int MaxRetries;
        internal WebSocketConnection Socket;

        private PlayerPacket Packet = new PlayerPacket();
        private List<RegisterRequest> RegisterCache = new List<RegisterRequest>();
        private ConcurrentQueue<RegisterRequest> RegisterQueue = new ConcurrentQueue<RegisterRequest>();
        private ConcurrentQueue<SubmitRequest> SubmitQueue = new ConcurrentQueue<SubmitRequest>();

        internal override bool BeginInitInternal()
        {
            if (Socket != null)
                EndInit();

            Socket = new WebSocketConnection(this, ID, Name, TryReconnect, MaxRetries);
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
                if (IsPlayerConnected())
                {
                    while (RegisterQueue.TryDequeue(out RegisterRequest request))
                        Packet.Register.Add(request);

                    while (SubmitQueue.TryDequeue(out SubmitRequest request))
                        Packet.Submit.Add(request);

                    Socket.Send(Packet);
                    Packet.Clear();
                }

                if (ShouldRun)
                    Thread.Sleep(1);
            }
        }

        private void RequestRegister(RegisterRequest request)
        {
            RegisterCache.Add(request);
            RegisterQueue.Enqueue(request);
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

        internal bool IsPlayerConnected() => Socket?.IsConnected() ?? false;

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

        internal bool IsPlaying(string key) => Socket?.LastResponse?.ActiveKeys?.ContainsValue(key) ?? false;
        
        internal bool IsPlayingAny() => (Socket?.LastResponse?.ActiveKeys?.Count > 0);

        internal void StopPlaying(string key) => SubmitQueue.Enqueue(new SubmitRequest { key = key, type = "turnOff" });
        internal void StopPlayingAll() => SubmitQueue.Enqueue(new SubmitRequest { type = "turnOffAll" });

        internal bool IsFeedbackRegistered(string key) => Socket?.LastResponse?.RegisteredKeys?.ContainsValue(key) ?? false;

        internal void Submit(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints, List<PathPoint> pathPoints)
        {
            if (dotPoints?.Count > bHapticsManager.MaxMotorsPerPositionType)
                return; // To-Do: Throw Exception

            if (position == PositionType.Vest)
            {
                Submit($"{key}Front", durationMillis, PositionType.VestFront, dotPoints, pathPoints);
                Submit($"{key}Back", durationMillis, PositionType.VestBack, dotPoints, pathPoints);
                return;
            }

            SubmitRequest request = new SubmitRequest { key = key, type = "frame" };
            request.Frame.durationMillis = durationMillis;
            request.Frame.position = position;
            if (dotPoints != null)
                request.Frame.dotPoints.AddRange(dotPoints);
            if (pathPoints != null)
                request.Frame.pathPoints.AddRange(pathPoints);
            SubmitQueue.Enqueue(request);
        }

        internal void Submit(string key, int durationMillis, PositionType position, DotPoint[] dotPoints, PathPoint[] pathPoints)
        {
            if (dotPoints?.Length > bHapticsManager.MaxMotorsPerPositionType)
                return; // To-Do: Throw Exception

            if (position == PositionType.Vest)
            {
                Submit($"{key}Front", durationMillis, PositionType.VestFront, dotPoints, pathPoints);
                Submit($"{key}Back", durationMillis, PositionType.VestBack, dotPoints, pathPoints);
                return;
            }

            SubmitRequest request = new SubmitRequest { key = key, type = "frame" };
            request.Frame.durationMillis = durationMillis;
            request.Frame.position = position;
            if (dotPoints != null)
                request.Frame.dotPoints.AddRange(dotPoints);
            if (pathPoints != null)
                request.Frame.pathPoints.AddRange(pathPoints);
            SubmitQueue.Enqueue(request);
        }

        internal void SubmitRegistered(string key) => SubmitQueue.Enqueue(new SubmitRequest { key = key, type = "key" });
        
        /*
        internal void SubmitRegistered(string key, float ratio)
        {
            SubmitRequest request = new SubmitRequest { key = key, type = "key" };
            request.Parameters["ratio"] = ratio;
            SubmitQueue.Enqueue(request);
        }
        */

        public void SubmitRegistered(string key, string altKey, ScaleOption scaleOption, RotationOption rotationOption)
        {
            SubmitRequest request = new SubmitRequest { key = key, type = "key" };
            request.Parameters["altKey"] = altKey;
            if (scaleOption != null)
                request.Parameters["scaleOption"] = scaleOption;
            if (rotationOption != null)
                request.Parameters["rotationOption"] = rotationOption;
            SubmitQueue.Enqueue(request);
        }
    }
}
