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
        public Client(string host, int port)
        {
            this.host = host;
            this.port = port;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            player = new Player()
            {
                Name = "New player",
            };
        }

        private int port;
        private string host;
        private Socket socket;
        
        private IPEndPoint serverEndPoint;
        private Player player;

        private bool working;


        public event Action OnConntected;
        public event Action OnDisconnected;


        public void Start()
        {
            try
            {
                Console.ReadKey(false);
                working = true;

                IPAddress serverIP = IPAddress.Parse(host);
                serverEndPoint = new IPEndPoint(serverIP, port);

                socket.Bind(new IPEndPoint(IPAddress.Any, 0));

                Task.Run(RecieveLoop);

                Send(new Message(nameof(ConnectMessage), player));

                while(working)
                {
                    string text = Console.ReadLine();

                    Message message = new Message(nameof(ChatMessage), player, text);
                    Send(message);
                }

                Logger.Log("Connecting...");
            }
            catch
            {
                Logger.Log("Can not connect.");
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
            await socket.SendToAsync(message.ToBytes(), SocketFlags.None, serverEndPoint);
        }


        public override void ConnectMessage(Player player, IPEndPoint endPoint)
        {
            Logger.Log("Connected.");

            OnConntected?.Invoke();
        }
        public override void DisconnectMessage(Player player, IPEndPoint endPoint)
        {
            working = false;
            socket.Close();

            Logger.Log("Disconnected.");

            OnDisconnected?.Invoke();
        }
        public override void ChatMessage(Player player, string text, IPEndPoint endPoint)
        {
            Logger.Log($"{player.Name}: {text}");
        }
    }
}
