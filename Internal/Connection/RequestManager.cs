using System.Collections.Concurrent;
using System.Threading;
using bHapticsLib.Internal.Connection.Models;

namespace bHapticsLib.Internal.Connection
{
    internal class RequestManager : ThreadedTask
    {
        private string ID, Name;
        private bool ShouldRun = true;

        internal bool TryReconnect = true;
        internal WebSocketConnection Socket;

        private PlayerPacket Packet = new PlayerPacket();
        internal ConcurrentQueue<RegisterRequest> RegisterQueue = new ConcurrentQueue<RegisterRequest>();
        private ConcurrentQueue<SubmitRequest> SubmitQueue = new ConcurrentQueue<SubmitRequest>();

        internal RequestManager(string id, string name)
        {
            ID = id;
            Name = name;
        }

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

        internal bool IsConnected()
            => Socket?.IsConnected ?? false;

        internal void StopPlaying(string key)
            => SubmitQueue.Enqueue(new SubmitRequest { key = key, type = "turnOff" });
        internal void StopPlayingAll()
            => SubmitQueue.Enqueue(new SubmitRequest { type = "turnOffAll" });
    }
}
