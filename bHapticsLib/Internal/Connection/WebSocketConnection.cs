using System;
using System.Timers;
using System.Web;
using WebSocketSharp;
using bHapticsLib.Internal.SimpleJSON;
using bHapticsLib.Internal.Connection.Models;

namespace bHapticsLib.Internal.Connection
{
    internal class WebSocketConnection : IDisposable
    {
        private string URL = $"ws://{bHapticsManager.IPAddress}:{bHapticsManager.Port}/{bHapticsManager.Endpoint}";
        private string ID, Name;
        private bool TryToReconnect;
        private int MaxRetries;

        private int RetryCount;
        private int RetryDelay = 3; // In Seconds
        private Timer RetryTimer;
        internal WebSocket Socket;

        internal bool IsSocketConnected;

        internal PlayerResponse LastResponse;

        //internal event Action ConnectionChanged;
        //internal event Action ResponseReceived;
        //internal event Action<object, ErrorEventArgs> OnError;

        private ConnectionManager Parent;

        internal WebSocketConnection(ConnectionManager parent, string id, string name, bool tryToReconnect, int maxRetries)
        {
            Parent = parent;
            ID = HttpUtility.UrlEncode(id.Replace(" ", "_"));
            Name = HttpUtility.UrlEncode(name.Replace(" ", "_"));
            TryToReconnect = tryToReconnect;
            MaxRetries = maxRetries;

            if (TryToReconnect)
            {
                RetryTimer = new Timer(RetryDelay * 1000); // S -> MS
                RetryTimer.Elapsed += (sender, args) => RetryCheck();
                RetryTimer.Start();
            }

            Socket = new WebSocket(URL + "?app_id=" + ID + "&app_name=" + Name);
            Socket.Log.Output = (LogData data, string msg) => { };
            Socket.EmitOnPing = true;

            //Socket.OnError += (sender, args) => OnError?.Invoke(sender, args);

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

                //ResponseReceived?.Invoke();
            };

            Socket.OnOpen += (sender, args) =>
            {
                RetryCount = 0;
                IsSocketConnected = true;

                Parent.QueueRegisterCache();

                //ConnectionChanged?.Invoke();
            };

            Socket.OnClose += (sender, args) =>
            {
                IsSocketConnected = false;
                LastResponse = null;

                //ConnectionChanged?.Invoke();
            };

            Socket.Connect();
        }

        public void Dispose()
        {
            try
            {
                Socket.Close();
                if (TryToReconnect)
                {
                    RetryTimer.Stop();
                    RetryTimer.Dispose();
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        private void RetryCheck()
        {
            if (IsConnected())
                return;
            if (!TryToReconnect)
                return;

            if (MaxRetries > 0)
            {
                if (RetryCount >= MaxRetries)
                {
                    Parent.EndInit();
                    return;
                }

                RetryCount++;
            }

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
                //Console.WriteLine($"Sent: {msg}");
            }
            catch (Exception e) { Console.Write($"{e.Message} {e}\n"); }
        }
    }
}