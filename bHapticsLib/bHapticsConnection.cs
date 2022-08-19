using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using bHapticsLib.Internal;
using bHapticsLib.Internal.Models.Connection;
using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib
{
    public class bHapticsConnection : ThreadedTask
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
        private IPAddress IPAddress = IPAddress.Loopback;
        private static int Port = 15881;
        private static string Endpoint = "v2/feedbacks";

        internal string ID, Name;
        internal bool TryToReconnect;
        internal int MaxRetries;
        internal WebSocketConnection Socket;
        private PlayerPacket Packet = new PlayerPacket();
        private bool ShouldRun = true;

        internal bHapticsConnection() { }
        public bHapticsConnection(string id, string name, bool tryToReconnect = true, int maxRetries = 5)
            : this(IPAddress.Loopback, id, name, tryToReconnect, maxRetries) { }
        public bHapticsConnection(IPAddress ipaddress, string id, string name, bool tryToReconnect = true, int maxRetries = 5)
        {
            ID = id;
            Name = name;
            TryToReconnect = tryToReconnect;
            MaxRetries = maxRetries.Clamp(0, int.MaxValue);
            IPAddress = ipaddress;
        }

        internal override bool BeginInitInternal()
        {
            if (Socket != null)
                EndInit();
            
            Socket = new WebSocketConnection(this, ID, Name, TryToReconnect, MaxRetries, IPAddress, Port, Endpoint);
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

                    if (!Packet.IsEmpty())
                    {
                        Socket.Send(Packet);
                        Packet.Clear();
                    }
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

        public bHapticsStatus Status
        {
            get => !IsAlive() ? bHapticsStatus.Disconnected
                : !IsConnected() ? bHapticsStatus.Connecting
                : bHapticsStatus.Connected;
        }

        #endregion

        #region Device
        public int GetConnectedDeviceCount() => Socket?.LastResponse?.ConnectedDeviceCount ?? 0;
        public bool IsDeviceConnected(PositionID type)
        {
            if ((type == PositionID.VestFront)
                || (type == PositionID.VestBack))
                type = PositionID.Vest;
            return Socket?.LastResponse?.ConnectedPositions?.ContainsValue(type.ToPacketString()) ?? false;
        }
        public int[] GetDeviceStatus(PositionID type)
        {
            if ((Socket == null)
                || (Socket.LastResponse == null))
                return default;

            JSONNode statusArray = Socket.LastResponse.Status;
            if (type == PositionID.Vest)
            {
                JSONNode frontStatus = statusArray[PositionID.VestFront.ToString()];
                JSONNode backStatus = statusArray[PositionID.VestBack.ToString()];

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
        public bool IsPlaying(string key) => Socket?.LastResponse?.ActiveKeys?.ContainsValue(key) ?? false;
        public bool IsPlayingAny() => (Socket?.LastResponse?.ActiveKeys?.Count > 0);
        #endregion

        #region StopPlaying
        public void StopPlaying(string key)
        {
            if (!IsAlive() || !IsConnected())
                return;
            SubmitQueue.Enqueue(new SubmitRequest { key = key, type = "turnOff" });
        }
        public void StopPlayingAll()
        {
            if (!IsAlive() || !IsConnected())
                return;
            SubmitQueue.Enqueue(new SubmitRequest { type = "turnOffAll" });
        }
        #endregion

        #region PatternRegister
        public bool IsPatternRegistered(string key) => Socket?.LastResponse?.RegisteredKeys?.ContainsValue(key) ?? false;

        public void RegisterPatternFromFile(string key, string tactFilePath)
        {
            if (!File.Exists(tactFilePath))
                return; // To-Do: Exception Here

            RegisterPatternFromJson(key, File.ReadAllText(tactFilePath));
        }

        public void RegisterPatternFromJson(string key, string tactFileStr)
        {
            if (string.IsNullOrEmpty(key))
                return; // To-Do: Exception Here

            if (string.IsNullOrEmpty(tactFileStr))
                return; // To-Do: Exception Here

            RegisterRequest request = new RegisterRequest();
            request.key = key;
            request.project = JSON.Parse(tactFileStr)[nameof(request.project)].AsObject;

            RegisterCache.Add(request);
            RegisterQueue.Enqueue(request);
        }

        public void RegisterPatternSwappedFromFile(string key, string tactFilePath)
        {
            if (!File.Exists(tactFilePath))
                return; // To-Do: Exception Here

            RegisterPatternSwappedFromJson(key, File.ReadAllText(tactFilePath));
        }

        public void RegisterPatternSwappedFromJson(string key, string tactFileStr)
        {
            if (string.IsNullOrEmpty(key))
                return; // To-Do: Exception Here

            if (string.IsNullOrEmpty(tactFileStr))
                return; // To-Do: Exception Here

            RegisterRequest request = new RegisterRequest();
            request.key = key;

            JSONObject project = JSON.Parse(tactFileStr)[nameof(project)].AsObject;
            JSONArray tracks = project[nameof(tracks)].AsArray;
            LoopTracks(tracks, (effect) =>
            {
                JSONNode modes = effect[nameof(modes)];
                JSONNode modeLeft = modes[0];
                JSONNode modeRight = modes[1];
                modes[0] = modeRight;
                modes[1] = modeLeft;
                effect[nameof(modes)] = modes;
            });
            project[nameof(tracks)] = tracks;

            request.project = project;

            RegisterCache.Add(request);
            RegisterQueue.Enqueue(request);
        }

        private static void LoopTracks(JSONArray tracks, Action<JSONObject> act)
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                JSONObject projectTrack = tracks[i].AsObject;
                JSONArray effects = projectTrack[nameof(effects)].AsArray;
                for (int i2 = 0; i2 < effects.Count; i2++)
                {
                    JSONObject projectEffect = effects[i2].AsObject;
                    act(projectEffect);
                    effects[i2] = projectEffect;
                }
                projectTrack[nameof(effects)] = effects;
                tracks[i] = projectTrack;
            }
        }
        #endregion

        #region Submit
        public void Submit<A, B>(
            string key,
            int durationMillis,
            PositionID position,
            A dotPoints,
            B pathPoints,
            MirrorDirection dotMirrorDirection = MirrorDirection.None)
            where A : IList, ICollection
            where B : IList<PathPoint>, ICollection<PathPoint>
        {
            if (!IsAlive())
                return;

            if (position == PositionID.Vest)
            {
                Submit($"{key}Front", durationMillis, PositionID.VestFront, dotPoints, pathPoints, dotMirrorDirection);
                Submit($"{key}Back", durationMillis, PositionID.VestBack, dotPoints, pathPoints, dotMirrorDirection);
                return;
            }

            SubmitRequest request = new SubmitRequest { key = key, type = "frame" };
            request.Frame.durationMillis = durationMillis;
            request.Frame.position = position.ToPacketString();

            if ((dotPoints != null) && (dotPoints.Count > 0))
            {
                object[] newDotPoints = null;
                if (dotMirrorDirection != MirrorDirection.None)
                {
                    newDotPoints = new object[dotPoints.Count];
                    for (int i = 0; i < dotPoints.Count; i++)
                        newDotPoints[i] = dotPoints[i];

                    switch (dotMirrorDirection)
                    {
                        case MirrorDirection.Horizontal:
                            MirrorHorizontal(ref newDotPoints, position);
                            goto default;

                        case MirrorDirection.Vertical:
                            MirrorVertical(ref newDotPoints, position);
                            goto default;

                        case MirrorDirection.Both:
                            MirrorHorizontal(ref newDotPoints, position);
                            MirrorVertical(ref newDotPoints, position);
                            goto default;

                        default:
                            break;
                    }
                }

                Type pointType = null;
                for (int i = 0; (i < ((newDotPoints == null) ? dotPoints.Count : newDotPoints.Length)); i++)
                {
                    object point = (newDotPoints == null) ? dotPoints[i] : newDotPoints[i];
                    if (point == null)
                        continue;

                    if (pointType == null)
                        pointType = point.GetType();

                    if ((pointType == intType) || (pointType == byteType))
                    {
                        JSONObject node = new JSONObject();
                        node["index"] = i.Clamp(0, bHapticsManager.MaxMotorsPerDotPoint);

                        if (pointType == intType)
                            node["intensity"] = Extensions.Clamp<int>((int)point, 0, bHapticsManager.MaxIntensityInInt);
                        else if (pointType == byteType)
                            node["intensity"] = Extensions.Clamp<byte>((byte)point, 0, bHapticsManager.MaxIntensityInByte);

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
        public void SubmitRegistered(string key, string altKey = null, ScaleOption scaleOption = null, RotationOption rotationOption = null)
        {
            if (!IsAlive())
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
        public void SubmitRegisteredMillis(string key, int startTimeMillis = 0)
        {
            if (!IsAlive())
                return;

            SubmitRequest request = new SubmitRequest { key = key, type = "key" };

            request.Parameters["startTimeMillis"] = startTimeMillis;

            SubmitQueue.Enqueue(request);
        }
        #endregion

        #region Mirror
        private static void MirrorHorizontal<A>(ref A dotPoints, PositionID position) where A : IList, ICollection
        {
            int fullCount = dotPoints.Count;
            int halfCount = fullCount / 2;

            if (fullCount != bHapticsManager.MaxMotorsPerDotPoint)
            {
                dotPoints.Reverse(0, fullCount);
                return;
            }

            switch (position)
            {
                case PositionID.Head:
                    dotPoints.Reverse(0, fullCount);
                    break;

                case PositionID.VestFront:
                case PositionID.VestBack:
                    dotPoints.Reverse(0, 4);
                    dotPoints.Reverse(4, 4);
                    dotPoints.Reverse(8, 4);
                    dotPoints.Reverse(12, 4);
                    dotPoints.Reverse(16, 4);
                    break;

                case PositionID.ArmLeft:
                case PositionID.ArmRight:
                case PositionID.FootLeft:
                case PositionID.FootRight:
                    dotPoints.Reverse(0, halfCount);
                    dotPoints.Reverse(halfCount + 1, fullCount);
                    break;

                default:
                    break;
            }
        }
        private static void MirrorVertical<A>(ref A dotPoints, PositionID position) where A : IList, ICollection
        {
            int fullCount = dotPoints.Count;

            if (fullCount != bHapticsManager.MaxMotorsPerDotPoint)
            {
                dotPoints.Reverse(0, fullCount);
                return;
            }

            switch (position)
            {
                case PositionID.VestFront:
                case PositionID.VestBack:
                    dotPoints.Swap(0, 16);
                    dotPoints.Swap(1, 17);
                    dotPoints.Swap(2, 18);
                    dotPoints.Swap(3, 19);
                    dotPoints.Swap(4, 12);
                    dotPoints.Swap(5, 13);
                    dotPoints.Swap(6, 14);
                    dotPoints.Swap(7, 15);
                    break;

                case PositionID.ArmLeft:
                case PositionID.ArmRight:
                    dotPoints.Swap(0, 3);
                    dotPoints.Swap(1, 4);
                    dotPoints.Swap(2, 5);
                    break;

                case PositionID.HandLeft:
                case PositionID.HandRight:
                    dotPoints.Reverse(0, fullCount);
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
