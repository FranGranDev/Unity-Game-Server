using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;
using Networking.Attributes;
using Networking.Services;
using Networking.Messages;
using Networking.Messages.Data;

namespace Networking.ServerSide
{
    public class Server : NetworkMethods
    {
        public Server()
        {
            socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Any, 8001);

            handlers = new Dictionary<IPEndPoint, Handler>();
        }

        private readonly Socket socket;
        private readonly IPEndPoint endPoint;
        private readonly Dictionary<IPEndPoint, Handler> handlers;

        private bool working;


        public event Action<Player> OnPlayerConnected;
        public event Action<Player> OnPlayerDisconnected;


        public void Start()
        {
            socket.Bind(endPoint);

            Logger.Log($"Server started: IP {endPoint}");

            working = true;
            Task.Run(RecieveLoop);
        }
        public void Stop()
        {
            socket.Close();
            working = false;
        }

        private async void RecieveLoop()
        {
            while(working)
            {
                await Recieve();
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
            IEnumerable<Handler> receivers = handlers.Values;

            foreach (Handler handler in receivers)
            {
                handler.Send(message);
            }
        }
        private void Broadcast(Message message, IPEndPoint except)
        {
            IEnumerable<Handler> receivers = handlers.Values
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

            Logger.Log($"Chat: {player.Name} | {text}");
        }
        public override void ConnectMessage(Player player, IPEndPoint endPoint)
        {
            Handler handler = new Handler(socket, endPoint, player);
            handlers.Add(endPoint, handler);

            Message message = new Message(nameof(ConnectMessage), player);
            handler.Send(message);

            Logger.Log($"New player connected: {player.Name} | {endPoint}");
        }
        public override void DisconnectMessage(Player player, IPEndPoint endPoint)
        {
            if(handlers.ContainsKey(endPoint))
            {
                handlers.Remove(endPoint);
            }

            Logger.Log($"Player disconnected: {player.Name}");
        }
    }
}
