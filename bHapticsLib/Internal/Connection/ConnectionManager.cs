using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using bHapticsLib.Internal.Connection.Models;

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
        internal bool IsDeviceConnected(PositionType type) => Socket?.LastResponse?.ConnectedPositions?.ContainsValue(type) ?? false;

        internal bool IsPlaying(string key) => Socket?.LastResponse?.ActiveKeys?.ContainsValue(key) ?? false;
        //internal bool IsPlaying(PositionType type) => Socket?.LastResponse?.Status?.ContainsValue(key) ?? false;
        internal bool IsPlayingAny() => (Socket?.LastResponse?.ActiveKeys?.Count > 0);

        internal void StopPlaying(string key) => SubmitQueue.Enqueue(new SubmitRequest { key = key, type = "turnOff" });
        internal void StopPlayingAll() => SubmitQueue.Enqueue(new SubmitRequest { type = "turnOffAll" });

        internal bool IsFeedbackRegistered(string key) => Socket?.LastResponse?.RegisteredKeys?.ContainsValue(key) ?? false;

        internal void Submit(string key, int durationMillis, PositionType position, List<DotPoint> dotPoints, List<PathPoint> pathPoints)
        {
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
    }
}
