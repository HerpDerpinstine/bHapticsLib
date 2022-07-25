using System;
using System.IO;
using System.Timers;
using SocketWrenchSharp;
using SocketWrenchSharp.Messages;
using SocketWrenchSharp.Protocol;
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

            CreateSocket();

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
                Socket = null;
                isConnected = false;
                if (TryToReconnect)
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

        internal void CreateSocket()
        {
            Socket = new WebSocket($"{URL}?app_id={ID}&app_name={Name}", false);

            Socket.MessageReceived += (msg) =>
            {
                try
                {
                    if (msg.ToFrame().Opcode != WebSocketOpcode.Text)
                        return;

                    if (LastResponse == null)
                        LastResponse = new PlayerResponse();

                    WebSocketTextMessage textMessage = msg as WebSocketTextMessage;

                    JSONNode node = JSON.Parse(textMessage.Text);
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
        }

        internal void TryConnect()
        {
            if (Socket == null)
                return;

            try
            {
                Socket.Connect();
            }
            catch
            {
                CreateSocket();
                // To-Do
            }
        }

        private void RetryCheck()
        {
            if (IsConnected() || !TryToReconnect)
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

        internal bool IsConnected() => isConnected && (Socket != null) && (Socket.State == WebSocketState.Open);

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
            catch (IOException ex)
            {
                isConnected = false;
            }
            catch
            {
                // To-Do
            }
        }
    }
}