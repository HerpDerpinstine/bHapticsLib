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
        private Timer UpTime;
        internal WebSocket Socket;

        internal bool IsSocketConnected;
        internal PlayerResponse LastResponse;

        internal event Action ConnectionChanged;
        internal event Action<string> LogReceived;
        internal event Action<object, ErrorEventArgs> OnError;

        internal WebSocketConnection(ConnectionManager manager, string id, string name, bool tryReconnect)
        {
            ID = HttpUtility.UrlEncode(id.Replace(" ", "_"));
            Name = HttpUtility.UrlEncode(name.Replace(" ", "_"));

            if (tryReconnect)
            {
                UpTime = new Timer(3 * 1000); // 3 sec
                UpTime.Elapsed += (sender, args) => Connect();
                UpTime.Start();
            }

            Socket = new WebSocket(URL + "?app_id=" + ID + "&app_name=" + Name);
            Socket.OnError += (sender, args) => OnError?.Invoke(sender, args);
            Socket.OnMessage += (sender, args) =>
            {
                string data = args.Data;
                Console.WriteLine($"Response: {data}");
                LastResponse = JSONNode.Parse(data) as PlayerResponse;
                LogReceived?.Invoke(data);
            };

            Socket.OnOpen += (sender, args) =>
            {
                IsSocketConnected = true;
                manager.QueueRegisterCache();
                ConnectionChanged?.Invoke();
            };

            Socket.OnClose += (sender, args) =>
            {
                IsSocketConnected = false;
                ConnectionChanged?.Invoke();
                LastResponse = null;
            };

            Connect();
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing Socket...");
            try
            {
                Socket.Close();
                UpTime.Stop();
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        private void Connect()
        {
            if (IsConnected())
                return;
            Socket.Connect();
        }

        internal bool IsConnected() => IsSocketConnected && (Socket != null) && (Socket.ReadyState != WebSocketState.Closing);

        internal void Send(JSONObject jsonNode)
            => Send(jsonNode.ToString());
        internal void Send(string msg)
        {
            if (!IsConnected())
                return;
            try
            {
                Socket.Send(msg);
                Debug.WriteLine("Sent: " + msg);
            }
            catch (Exception e) { Console.Write($"{e.Message} {e}\n"); }
        }
    }
}
