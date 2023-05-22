using UnityEngine;
using System.Collections;

using Networking.Messages;
using Networking.Messages.Data;
using Networking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


namespace Networking.ServerSide
{
    public class Server : NetworkMethods
    {
        public Server(int port)
        {
            socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            EndPoint = new IPEndPoint(IPAddress.Any, port);

            Handlers = new Dictionary<IPEndPoint, Handler>();
        }

        private readonly Socket socket;
        private Thread recieveThread;

        public Dictionary<IPEndPoint, Handler> Handlers { get; }
        public IPEndPoint EndPoint { get; }
        public bool Working { get; private set; }



        public void Start()
        {
            socket.Bind(EndPoint);

            UIDebugger.Log($"Server started: IP {EndPoint}");

            Working = true;

            recieveThread = new Thread(new ThreadStart(RecieveLoop));
            recieveThread.IsBackground = true;
            recieveThread.Start();
        }
        public void Stop()
        {
            Message message = new Message(nameof(DisconnectMessage), Player.ServerPlayer);

            Broadcast(message);

            recieveThread.Abort();
            socket.Close();
            Working = false;
        }

        private async void RecieveLoop()
        {
            while (Working)
            {
                UIDebugger.Log($"Wait for package...");
                await Recieve();
                UIDebugger.Log($"Recieve package...");
            }
        }
        private async Task Recieve()
        {
            byte[] buffer = new byte[1024];
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            var info = await socket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, remoteEndPoint);

            byte[] data = new byte[info.ReceivedBytes];
            Array.Copy(buffer, data, info.ReceivedBytes);


            Message message = Message.FromBytes(data);
            Invoker.Invoke(message.MethodName, Concat(message.GetData(), info.RemoteEndPoint));
        }


        private void Broadcast(Message message)
        {
            IEnumerable<Handler> receivers = Handlers.Values;

            foreach (Handler handler in receivers)
            {
                handler.Send(message);
            }
        }
        private void Broadcast(Message message, IPEndPoint except)
        {
            IEnumerable<Handler> receivers = Handlers.Values
                .Where(x => !x.EndPoint.Equals(except));

            foreach (Handler handler in receivers)
            {
                handler.Send(message);
            }
        }


        public override void ChatMessage(Player player, string text, IPEndPoint endPoint)
        {
            Message message = new Message(nameof(ChatMessage), player, text);

            Broadcast(message, endPoint);

            UIDebugger.Log($"Chat: {player.Name} | {text}");
        }
        public override void ConnectMessage(Player player, IPEndPoint endPoint)
        {
            Handler handler = new Handler(socket, endPoint, player);
            Handlers.Add(endPoint, handler);

            Message message = new Message(nameof(ConnectMessage), player);
            Broadcast(message);

            UIDebugger.Log($"New player connected: {player.Name} | {endPoint}");
        }
        public override void DisconnectMessage(Player player, IPEndPoint endPoint)
        {
            if (Handlers.ContainsKey(endPoint))
            {
                Handlers.Remove(endPoint);
            }

            Message message = new Message(nameof(DisconnectMessage), player);
            Broadcast(message, endPoint);

            UIDebugger.Log($"Player disconnected: {player.Name}");
        }
    }
}
