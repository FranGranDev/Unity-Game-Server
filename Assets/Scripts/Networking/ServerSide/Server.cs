using UnityEngine;
using System.Collections;

using Networking.Messages;
using Networking.Data;
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
            EndPoint = new IPEndPoint(IPAddress.Any, port);
            udpClient = new UdpClient(EndPoint);

            Handlers = new Dictionary<IPEndPoint, Handler>();
        }

        private UdpClient udpClient;
        private Thread recieveThread;

        public Dictionary<IPEndPoint, Handler> Handlers { get; }
        public IPEndPoint EndPoint { get; }
        public bool Working { get; private set; }



        public void Start()
        {
            SafeDebugger.Log($"Server started: IP {EndPoint}");

            Working = true;


            recieveThread = new Thread(new ThreadStart(RecieveUDPLoop));
            recieveThread.IsBackground = true;
            recieveThread.Start();
        }
        public async void Stop()
        {
            Message message = new Message(nameof(DisconnectMessage), Player.ServerPlayer);

            await Broadcast(message);

            recieveThread.Abort();

            udpClient.Close();
            udpClient = null;

            Working = false;

            Handlers.Clear();
        }


        private async void RecieveUDPLoop()
        {
            while (Working)
            {
                await RecieveUDP();
            }

        }
        private async Task RecieveUDP()
        {
            UdpReceiveResult receivedResult = await udpClient.ReceiveAsync();

            byte[] data = new byte[receivedResult.Buffer.Length];
            Array.Copy(receivedResult.Buffer, data, receivedResult.Buffer.Length);


            Message message = Message.FromBytes(data);
            RecieveInfo info = new RecieveInfo()
            {
                EndPoint = receivedResult.RemoteEndPoint,
                MessageId = message.Id,
            };
            Invoker.Invoke(message.MethodName, Concat(message.GetData(), info));
        }


        private async Task Broadcast(Message message)
        {
            IEnumerable<Handler> receivers = new List<Handler>(Handlers.Values);

            foreach (Handler handler in receivers)
            {
                await handler.Send(message);
            }
        }
        private async Task Broadcast(Message message, IPEndPoint except)
        {
            IEnumerable<Handler> receivers = new List<Handler>(Handlers.Values
                .Where(x => !x.EndPoint.Equals(except)));

            foreach (Handler handler in receivers)
            {
                await handler.Send(message);
            }
        }


        public override async void ConnectMessage(Player player, RecieveInfo info)
        {
            Handler handler = new Handler(udpClient, info.EndPoint);
            handler.Player = player;
            Handlers.Add(info.EndPoint, handler);

            Message message = new Message(nameof(ConnectMessage), player);
            message.Id = info.MessageId;

            await Broadcast(message);

            SafeDebugger.Log($"Server | New player connected: {player.Name} | {info.EndPoint}");
        }
        public override async void DisconnectMessage(Player player, RecieveInfo info)
        {
            if (!Handlers.ContainsKey(info.EndPoint))
            {
                return;
            }
            Handlers.Remove(info.EndPoint);

            Message message = new Message(nameof(DisconnectMessage), player);
            message.Id = info.MessageId;

            await Broadcast(message, info.EndPoint);

            SafeDebugger.Log($"Server | Player disconnected: {player.Name}");
        }
        public override async void ChatMessage(Player player, string text, RecieveInfo info)
        {
            Message message = new Message(nameof(ChatMessage), player, text);
            message.Id = info.MessageId;

            await Broadcast(message, info.EndPoint);

            SafeDebugger.Log($"Server | Chat: {player.Name} | {text}");
        }

        public override async void RequestPlayersList(RecieveInfo info)
        {
            List<Player> players = Handlers
                .Select(x => x.Value.Player)
                .ToList();

            Message message = new Message(nameof(PlayersListMessage), players);
            message.Id = info.MessageId;

            if(Handlers.ContainsKey(info.EndPoint))
            {
                await Handlers[info.EndPoint].Send(message);
            }
        }

        public override async void LoadSceneMessage(int sceneIndex, RecieveInfo info)
        {
            Message message = new Message(nameof(LoadSceneMessage), sceneIndex);
            message.Id = info.MessageId;

            await Broadcast(message);
        }
    }
}
