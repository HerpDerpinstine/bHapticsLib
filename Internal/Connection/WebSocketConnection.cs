using System;
using System.Diagnostics;
using System.Timers;
using System.Web;
using WebSocketSharp;
using bHapticsLib.SimpleJSON;
using bHapticsLib.Internal.Connection.Models;

namespace bHapticsLib.Internal.Connection
{
    internal class WebSocketConnection : IDisposable
    {
        private readonly string URL = "ws://127.0.0.1:15881/v2/feedbacks";
        private string ID, Name;

        private WebSocket Socket;
        private Timer UpTime;

        private int RetryCount;
        private static int MaxRetryCount = 5;

        internal bool IsConnected { get; private set; }

        internal event Action<bool> ConnectionChanged;
        internal event Action<string> LogReceived;
        internal event Action<object, ErrorEventArgs> OnError;

        internal PlayerResponse LastResponse;

        internal WebSocketConnection(string id, string name, bool tryReconnect = true)
        {
            ID = HttpUtility.UrlEncode(id.Replace(" ", "_"));
            Name = HttpUtility.UrlEncode(name.Replace(" ", "_"));

            if (tryReconnect)
            {
                UpTime = new Timer(3 * 1000); // 3 sec
                UpTime.Elapsed += (sender, args) => UpTimeElapsed();
                UpTime.Start();
            }

            Socket = new WebSocket(URL + "?app_id=" + ID + "&app_name=" + Name);
            Socket.OnError += (sender, args) => OnError?.Invoke(sender, args);
            Socket.OnMessage += (sender, args) =>
            {
                LastResponse = (PlayerResponse)JSONNode.Parse(args.Data);
            };

            Socket.OnOpen += (sender, args) =>
            {
                IsConnected = true;

                //AddRegister(_registered);

                Console.WriteLine($"bHapticsLib Connected!");
                ConnectionChanged?.Invoke(IsConnected);
            };

            Socket.OnClose += (sender, args) =>
            {
                IsConnected = false;
                Console.WriteLine($"bHapticsLib Disconnected!");
                ConnectionChanged?.Invoke(IsConnected);
                LastResponse = null;
            };


            Socket.Connect();
        }

        public void Dispose()
        {
            try { Socket.Close(); }
            catch (Exception e) { Console.WriteLine(e); }
        }

        private void UpTimeElapsed()
        {
            if (IsConnected)
                return;

            RetryCount++;
            Socket.Connect();

            if (RetryCount >= MaxRetryCount)
                UpTime.Stop();
        }

        internal void Send(JSONObject jsonNode)
            => Send(jsonNode.Value);
        internal void Send(string msg)
        {
            if (!IsConnected)
                return;
            try
            {
                Socket.Send(msg);
                Debug.WriteLine("Send(string): " + msg);
            }
            catch (Exception e) { Console.Write($"{e.Message} {e}\n"); }
        }
    }
}
