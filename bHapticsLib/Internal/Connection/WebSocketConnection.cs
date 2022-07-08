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
        private int RetryCount;
        private Timer UpTime;
        internal WebSocket Socket;

        internal bool IsSocketConnected;

        internal PlayerResponse LastResponse;

        internal event Action ConnectionChanged;
        internal event Action ResponseReceived;
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
            Socket.Log.Output = (LogData data, string msg) => { };
            Socket.EmitOnPing = true;

            Socket.OnError += (sender, args) => OnError?.Invoke(sender, args);

            Socket.OnMessage += (sender, args) =>
            {
                try
                {
                    if (LastResponse == null)
                        LastResponse = new PlayerResponse();
                    
                    JSONNode node = JSON.Parse(args.Data);
                    if ((node == null) || node.IsNull || !node.IsObject)
                        return;

                    LastResponse.m_Dict = node.AsObject.m_Dict;
                }
                catch (Exception e) { Console.WriteLine(e); }

                ResponseReceived?.Invoke();
            };

            Socket.OnOpen += (sender, args) =>
            {
                RetryCount = 0;
                IsSocketConnected = true;

                manager.QueueRegisterCache();

                ConnectionChanged?.Invoke();
            };

            Socket.OnClose += (sender, args) =>
            {
                IsSocketConnected = false;
                LastResponse = null;

                ConnectionChanged?.Invoke();
            };

            Socket.Connect();
        }

        public void Dispose()
        {
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
            if (RetryCount >= bHapticsManager.MaxConnectionRetryCount)
            {
                UpTime.Stop();
                return;
            }

            RetryCount++;
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
                //Console.WriteLine("Sent: " + msg);
            }
            catch (Exception e) { Console.Write($"{e.Message} {e}\n"); }
        }
    }
}
