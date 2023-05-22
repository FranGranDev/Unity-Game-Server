using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking.Messages;
using Networking.Messages.Data;
using Networking.Services;


namespace Networking.ClientSide
{
    public class Client : NetworkMethods
    {
        public Client(Player player, IPAddress host, int port)
        {           
            this.player = player;
            this.host = host;
            this.port = port;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        private int port;
        private IPAddress host;
        private Socket socket;
        
        private IPEndPoint serverEndPoint;
        private Player player;

        private bool working;


        public event Action<Player> OnPlayerConntected;
        public event Action<Player> OnPlayerDisconnected;


        public void Start()
        {
            try
            {
                working = true;

                serverEndPoint = new IPEndPoint(host, port);

                socket.Bind(new IPEndPoint(IPAddress.Any, 0));

                Task.Run(RecieveLoop);

                Send(new Message(nameof(ConnectMessage), player));

                Logger.Log($"Connecting...");
            }
            catch(Exception e)
            {
                Logger.Log($"Can not connect: {e}");
            }
        }
        public void Stop()
        {
            Send(new Message(nameof(DisconnectMessage), player));
            DisconnectMessage(player, serverEndPoint);
        }

        private async void RecieveLoop()
        {
            try
            {
                while (working)
                {
                    await Recieve();
                }
            }
            catch
            {
                DisconnectMessage(player, serverEndPoint);
            }
        }
        private async Task Recieve()
        {
            byte[] buffer = new byte[1024];
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            var info = await socket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, remoteEndPoint);

            byte[] data = new byte[info.ReceivedBytes];
            Array.Copy(buffer, data, info.ReceivedBytes);


            Message message = Message.FromBytes(data);
            Invoker.Invoke(message.MethodName, Concat(message.GetData(), info.RemoteEndPoint));
        }
        public async void Send(Message message)
        {
            Logger.Log($"Send: {message.MethodName} {serverEndPoint}");

            await socket.SendToAsync(message.ToBytes(), SocketFlags.None, serverEndPoint);
        }


        public override void ConnectMessage(Player player, IPEndPoint endPoint)
        {            
            OnPlayerConntected?.Invoke(player);
        }
        public override void DisconnectMessage(Player player, IPEndPoint endPoint)
        {
            working = false;
            socket.Close();

            OnPlayerDisconnected?.Invoke(player);

            Logger.Log("Disconnected.");
        }
        public override void ChatMessage(Player player, string text, IPEndPoint endPoint)
        {
            Logger.Log($"{player.Name}: {text}");
        }
    }
}
