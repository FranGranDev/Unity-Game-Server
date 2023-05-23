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

            serverEndPoint = new IPEndPoint(host, port);

            udpClient = new UdpClient();
        }

        private UdpClient udpClient;
        
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

                Task.Run(RecieveLoop);

                Send(new Message(nameof(ConnectMessage), player));

                SafeDebugger.Log($"Connecting...");
            }
            catch(Exception e)
            {
                SafeDebugger.Log($"Can not connect: {e}");
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
            catch(Exception e)
            {
                SafeDebugger.Log($"Exception: {e}");
                DisconnectMessage(player, serverEndPoint);
            }
        }
        private async Task Recieve()
        {
            UdpReceiveResult receivedResult = await udpClient.ReceiveAsync();

            byte[] data = new byte[receivedResult.Buffer.Length];
            Array.Copy(receivedResult.Buffer, data, receivedResult.Buffer.Length);


            Message message = Message.FromBytes(data);
            Invoker.Invoke(message.MethodName, Concat(message.GetData(), receivedResult.RemoteEndPoint));
        }
        public async void Send(Message message)
        {
            if (!working)
                return;

            byte[] data = message.ToBytes();
            await udpClient.SendAsync(data, data.Length, serverEndPoint);

            SafeDebugger.Log($"Send: {message.MethodName} {serverEndPoint}");
        }


        public override void ConnectMessage(Player player, IPEndPoint endPoint)
        {            
            OnPlayerConntected?.Invoke(player);
        }
        public override void DisconnectMessage(Player player, IPEndPoint endPoint)
        {
            working = false;
            udpClient.Close();

            OnPlayerDisconnected?.Invoke(player);

            SafeDebugger.Log("Disconnected.");
        }
        public override void ChatMessage(Player player, string text, IPEndPoint endPoint)
        {
            SafeDebugger.Log($"{player.Name}: {text}");
        }
    }
}
