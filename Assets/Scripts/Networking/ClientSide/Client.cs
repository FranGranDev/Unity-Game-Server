using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking.Messages;
using Networking.Data;
using Networking.Services;
using System.Collections.Generic;

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
        public event Action<Player, string> OnChatMessage;

        public event Action<object[], string> OnRecieveData;
        public event Action<string, object> OnUpdateObject;

        public event Action<int> OnLoadScene;


        public async void Start()
        {
            try
            {
                SafeDebugger.Log($"Client | Connecting...");
                working = true;

                Task recieve = Task.Run(RecieveLoop);

                await Send(new Message(nameof(ConnectMessage), player));
            }
            catch(Exception e)
            {
                SafeDebugger.Log($"Client | Can not connect: {e}");
            }
        }
        public async void Stop()
        {
            if (!working)
                return;

            await Send(new Message(nameof(DisconnectMessage), player));
            DisconnectMessage(player, new RecieveInfo() { EndPoint = serverEndPoint});

            working = false;
            try
            {
                udpClient.Close();
                udpClient = null;
            }
            catch { }
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
                Stop();
            }
        }
        private async Task Recieve()
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
        public async Task Send(Message message)
        {
            if (!working || udpClient == null)
                return;

            try
            {
                byte[] data = message.ToBytes();
                await udpClient.SendAsync(data, data.Length, serverEndPoint);
            }
            catch(Exception e)
            {
                SafeDebugger.Log($"Client | Exception: {e}");
            }
        }


        public override void ConnectMessage(Player player, RecieveInfo info)
        {            
            OnPlayerConntected?.Invoke(player);

            SafeDebugger.Log($"Client | Player: {player.Name} Connected.");
        }
        public override void DisconnectMessage(Player player, RecieveInfo info)
        {
            if(player.Server)
            {
                SafeDebugger.Log($"Client | Server stopped");
                Stop();
                return;
            }

            OnPlayerDisconnected?.Invoke(player);      
            
            SafeDebugger.Log($"Client | Player: {player.Name} Disconnected.");
        }
        public override void ChatMessage(Player player, string text, RecieveInfo info)
        {
            OnChatMessage?.Invoke(player, text);

            SafeDebugger.Log($"Client | {player.Name}: {text}");
        }

        public override void LoadSceneMessage(int sceneIndex, RecieveInfo info)
        {
            OnLoadScene?.Invoke(sceneIndex);
        }

        public override void PlayersListMessage(List<Player> players, RecieveInfo info)
        {
            OnRecieveData?.Invoke(new object[1] { players }, info.MessageId);
        }


        public override void UpdateObject(string id, object data, RecieveInfo info)
        {
            OnUpdateObject?.Invoke(id, data);
        }
    }
}
