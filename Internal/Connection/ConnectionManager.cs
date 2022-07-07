using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using bHapticsLib.Internal.Connection.Models;
using bHapticsLib.SimpleJSON;

namespace bHapticsLib.Internal.Connection
{
    internal class ConnectionManager : ThreadedTask
    {
        private bool ShouldRun = true;

        internal string ID, Name;
        internal bool TryReconnect = true;
        internal WebSocketConnection Socket;

        private PlayerPacket Packet = new PlayerPacket();
        internal ConcurrentQueue<RegisterRequest> RegisterQueue = new ConcurrentQueue<RegisterRequest>();
        private ConcurrentQueue<SubmitRequest> SubmitQueue = new ConcurrentQueue<SubmitRequest>();

        internal override bool BeginInitInternal()
        {
            if (Socket != null)
                EndInit();

            Socket = new WebSocketConnection(ID, Name, TryReconnect);
            ShouldRun = true;
            return true;
        }

        internal override bool EndInitInternal()
        {
            if (Socket == null)
                return false;

            ShouldRun = false;
            while (IsAlive()) { Thread.Sleep(1); }
            return true;
        }

        internal override void WithinThread()
        {
            while (ShouldRun)
            {
                if (Socket.IsConnected)
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
                else
                {
                    Socket.Dispose();
                    Socket = null;
                }
            }
        }

        internal bool IsConnected() => Socket?.IsConnected ?? false;
        internal bool IsDeviceConnected(PositionType type)
        {
            if ((Socket == null) || (Socket.LastResponse == null))
                return false;
            JSONNode.Enumerator enumerator = Socket.LastResponse.ConnectedPositions.GetEnumerator();
            while (enumerator.MoveNext())
                if ((PositionType)enumerator.Current.Value?.AsInt == type)
                    return true;
            return false;
        }
        internal bool IsAnyDeviceConnected() => (Socket?.LastResponse?.ConnectedDeviceCount > 0);

        internal bool IsPlaying(string key) => Socket?.LastResponse?.ActiveKeys.HasKey(key) ?? false;
        //internal bool IsPlaying(PositionType type) => Socket?.LastResponse?.ActiveKeys.HasKey(key) ?? false;
        internal bool IsPlayingAny() => (Socket?.LastResponse?.ActiveKeys.Count > 0);

        internal void StopPlaying(string key) => SubmitQueue.Enqueue(new SubmitRequest { key = key, type = "turnOff" });
        internal void StopPlayingAll() => SubmitQueue.Enqueue(new SubmitRequest { type = "turnOffAll" });
    }
}
