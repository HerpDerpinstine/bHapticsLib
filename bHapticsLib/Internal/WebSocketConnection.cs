using System;
using System.Timers;
using WebSocketDotNet;
using WebSocketDotNet.Messages;
using bHapticsLib.Internal.Models.Connection;
using bHapticsLib.Internal.SimpleJSON;

namespace bHapticsLib.Internal
{
    internal class WebSocketConnection : IDisposable
    {
        private bHapticsConnection Parent;
        
        internal bool FirstTry;
        private bool isConnected;

        private int RetryCount;
        private int RetryDelay = 3; // In Seconds
        private Timer RetryTimer;

        internal WebSocket Socket;
        internal PlayerResponse LastResponse;

        internal WebSocketConnection(bHapticsConnection parent)
        {
            Parent = parent;

            string URL = $"ws://{parent._ipaddress}:{bHapticsConnection.Port}/{bHapticsConnection.Endpoint}?app_id={parent.ID}&app_name={parent.Name}";
            Socket = new WebSocket(URL, false);

            Socket.TextReceived += (txt) =>
            {
                try
                {
                    if (LastResponse == null)
                        LastResponse = new PlayerResponse();

                    JSONNode node = JSON.Parse(txt);
                    if ((node == null) || node.IsNull || !node.IsObject)
                        return;

                    LastResponse.m_Dict = node.AsObject.m_Dict;
                }
                catch
                {
                    // To-Do
                }
            };

            Socket.Opened += () =>
            {
                isConnected = true;
                RetryCount = 0;
                Parent.QueueRegisterCache();
            };

            /*
            Socket.Closing += (closeCode, msg) =>
            {

            };
            */

            Socket.Closed += (closeCode, msg) =>
            {
                isConnected = false;
                LastResponse = null;
            };

            if (parent.TryToReconnect)
            {
                RetryTimer = new Timer(RetryDelay * 1000); // S -> MS
                RetryTimer.AutoReset = true;
                RetryTimer.Elapsed += (sender, args) => RetryCheck();
                RetryTimer.Start();
            }

            FirstTry = true;
        }

        public void Dispose()
        {
            try
            {
                Socket.SendClose();
                isConnected = false;
                if (Parent.TryToReconnect)
                {   
                    RetryTimer.Stop();
                    RetryTimer.Dispose();
                }
            }
            catch
            {
                // To-Do
            }
        }

        internal void TryConnect()
        {
            try
            {
                Socket.Connect();
            }
            catch
            {
                // To-Do
            }
        }

        private void RetryCheck()
        {
            if (IsConnected() || !Parent.TryToReconnect)
                return;

            if ((Socket.State == WebSocketState.Connecting) 
                || (Socket.State == WebSocketState.Closing))
                return;

            if (Parent.MaxRetries > 0)
            {
                if (RetryCount >= Parent.MaxRetries)
                {
                    Parent.EndInit();
                    return;
                }
                RetryCount++;
            }

            TryConnect();
        }

        internal bool IsConnected() => isConnected && (Socket.State == WebSocketState.Open);

        internal void Send(JSONObject jsonNode)
            => Send(jsonNode.ToString());
        internal void Send(string msg)
        {
            if (!IsConnected())
                return;

            try
            {
                Socket.Send(new WebSocketTextMessage(msg));
            }
            catch
            {
                // To-Do
            }
        }
    }
}