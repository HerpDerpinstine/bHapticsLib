using System;
using System.Timers;
using System.Web;
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

        private int RetryCount;
        private int RetryDelay = 3; // In Seconds
        private Timer RetryTimer;
        internal WebSocket Socket;

        internal bool IsSocketConnected;

        internal PlayerResponse LastResponse;

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
                catch (Exception e) { Console.WriteLine(e); }
            };

            Socket.Opened += () =>
            {
                RetryCount = 0;
                IsSocketConnected = true;
                Parent.QueueRegisterCache();
            };

            Socket.Closed += (closeCode, msg) =>
            {
                IsSocketConnected = false;
                LastResponse = null;
            };

            Socket.Connect();
        }

        public void Dispose()
        {
            try
            {
                Socket.SendClose();
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

            Socket.Connect();
        }

        internal bool IsConnected() => IsSocketConnected && (Socket != null) && (Socket.State != WebSocketState.Closing);

        internal void Send(JSONObject jsonNode)
            => Send(jsonNode.ToString());
        internal void Send(string msg)
        {
            if (!IsConnected())
                return;
            try
            {
                Socket.Send(new WebSocketTextMessage(msg));
                //Console.WriteLine($"Sent: {msg}");
            }
            catch (Exception e) { Console.Write($"{e.Message} {e}\n"); }
        }
    }
}