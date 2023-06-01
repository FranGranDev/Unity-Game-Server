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


namespace Networking.Server
{
    public class Server : NetworkMethods
    {
        private UdpClient udpClient;
        private Thread recieveThread;

        public Dictionary<IPEndPoint, Handler> Handlers { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public bool Working { get; private set; }



        public void Start(int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Any, port);
            udpClient = new UdpClient(EndPoint);

            Handlers = new Dictionary<IPEndPoint, Handler>();

            SafeDebugger.Log($"Server started: IP {EndPoint}");

            Working = true;


            recieveThread = new Thread(new ThreadStart(RecieveUDPLoop));
            recieveThread.IsBackground = true;
            recieveThread.Start();
        }
        public async Task Stop()
        {
            if (!Working)
                return;

            Message message = new Message(nameof(Disconnect), Player.ServerPlayer);

            await Broadcast(message);

            recieveThread.Abort();

            try
            {
                udpClient.Close();
            }
            finally
            {
                udpClient = null;
            }

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


        public override async void Connect(Player player, RecieveInfo info)
        {
            if(Handlers.Count >= 2)
            {
                return;
            }

            Handler handler = new Handler(udpClient, info.EndPoint);
            handler.Player = player;
            Handlers.Add(info.EndPoint, handler);

            Message message = new Message(nameof(Connect), player);
            message.Id = info.MessageId;

            await Broadcast(message);

            SafeDebugger.Log($"Server | New player connected: {player.Name} | {info.EndPoint}");
        }
        public override async void Disconnect(Player player, RecieveInfo info)
        {
            if (!Handlers.ContainsKey(info.EndPoint))
            {
                return;
            }
            Handlers.Remove(info.EndPoint);

            Message message = new Message(nameof(Disconnect), player);
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

            Message message = new Message(nameof(PlayersList), players);
            message.Id = info.MessageId;

            if(Handlers.ContainsKey(info.EndPoint))
            {
                await Handlers[info.EndPoint].Send(message);
            }
        }

        public override async void LoadScene(int sceneIndex, RecieveInfo info)
        {
            Message message = new Message(nameof(LoadScene), sceneIndex);
            message.Id = info.MessageId;

            await Broadcast(message);
        }
        public override async void StartRound(Dictionary<string, int> score, RecieveInfo info)
        {
            Message message = new Message(nameof(StartRound), score);

            await Broadcast(message);
        }
        public override async void EndRound(Player looser, RecieveInfo info)
        {
            Message message = new Message(nameof(EndRound), looser);

            await Broadcast(message);
        }


        public override async void UpdateObject(string id, object data, RecieveInfo info)
        {
            Message message = new Message(nameof(UpdateObject), id, data);

            await Broadcast(message, info.EndPoint);
        }
    }
}
