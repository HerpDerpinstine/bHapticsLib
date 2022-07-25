using System;
using System.Timers;
using WebSocketDotNet;
using WebSocketDotNet.Messages;
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

        internal bool FirstTry;
        private bool isConnected;
        private int RetryCount;
        private int RetryDelay = 3; // In Seconds
        private Timer RetryTimer;
        internal WebSocket Socket;

        internal PlayerResponse LastResponse;

        private ConnectionManager Parent;

        internal WebSocketConnection(ConnectionManager parent, string id, string name, bool tryToReconnect, int maxRetries)
        {
            Parent = parent;

            ID = id.Replace(" ", "_");
            Name = name.Replace(" ", "_");

            TryToReconnect = tryToReconnect;
            MaxRetries = maxRetries;

            Socket = new WebSocket($"{URL}?app_id={ID}&app_name={Name}", false);

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

            Socket.Closed += (closeCode, msg) =>
            {
                isConnected = false;
                LastResponse = null;
            };

            if (TryToReconnect)
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
                if (TryToReconnect)
                {   
                    RetryTimer.Stop();
                    RetryTimer.Dispose();
                }
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                // To-Do
            }
        }

        private void RetryCheck()
        {
            if (IsConnected() || !TryToReconnect)
                return;

            if ((Socket.State == WebSocketState.Connecting) 
                || (Socket.State == WebSocketState.Closing))
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
            catch (Exception ex)
            {
                // To-Do
            }
        }
    }
}