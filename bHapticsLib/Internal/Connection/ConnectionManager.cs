using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using bHapticsLib.Internal.Connection.Models;

namespace bHapticsLib.Internal.Connection
{
    internal class ConnectionManager : ThreadedTask
    {
        private bool ShouldRun = true;

        internal string ID, Name;
        internal bool TryReconnect = true;
        internal WebSocketConnection Socket;

        private PlayerPacket Packet = new PlayerPacket();
        private List<RegisterRequest> RegisterCache = new List<RegisterRequest>();
        private ConcurrentQueue<RegisterRequest> RegisterQueue = new ConcurrentQueue<RegisterRequest>();
        private ConcurrentQueue<SubmitRequest> SubmitQueue = new ConcurrentQueue<SubmitRequest>();

        internal override bool BeginInitInternal()
        {
            if (Socket != null)
                EndInit();

            Socket = new WebSocketConnection(this, ID, Name, TryReconnect);
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
            if (RegisterCache.FirstOrDefault(x => x.key.Equals(request.key)) != null)
                return; // To-Do: Exception Here

            RegisterCache.Add(request);
            RegisterQueue.Enqueue(request);
        }

        internal void QueueRegisterCache()
        {
            if (RegisterCache.Count <= 0)
                return;
            List<RegisterRequest>.Enumerator enumerator = RegisterCache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RegisterRequest request = enumerator.Current;
                if ((request == null)
                    || request.IsNull)
                    continue;
                RegisterQueue.Enqueue(request);
            }
            enumerator.Dispose();
        }

        internal bool IsPlayerConnected() => Socket?.IsConnected() ?? false;

        internal int GetConnectedDeviceCount()
        {
            if (Socket == null)
            {
                Console.WriteLine("Socket is null");
                return 0;
            }
            if (Socket.LastResponse == null)
            {
                Console.WriteLine("LastResponse is null");
                return 0;
            }

            Console.WriteLine("Found connected device count");
            return Socket.LastResponse.ConnectedDeviceCount;
        }
        internal bool IsDeviceConnected(PositionType type) => Socket?.LastResponse?.ConnectedPositions?.ContainsValue(type) ?? false;

        internal bool IsPlaying(string key) => Socket?.LastResponse?.ActiveKeys?.ContainsValue(key) ?? false;
        //internal bool IsPlaying(PositionType type) => Socket?.LastResponse?.ActiveKeys.HasKey(key) ?? false;
        internal bool IsPlayingAny() => (Socket?.LastResponse?.ActiveKeys?.Count > 0);

        internal void StopPlaying(string key) => SubmitQueue.Enqueue(new SubmitRequest { key = key, type = "turnOff" });
        internal void StopPlayingAll() => SubmitQueue.Enqueue(new SubmitRequest { type = "turnOffAll" });

        internal bool IsFeedbackRegistered(string key) => Socket?.LastResponse?.RegisteredKeys?.ContainsValue(key) ?? false;

    }
}
