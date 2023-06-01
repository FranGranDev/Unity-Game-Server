using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking.Messages;
using Networking.Data;
using Networking.Services;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace Networking.Server
{
    public class Client : NetworkMethods
    {
        private UdpClient udpClient;
        
        private IPEndPoint serverEndPoint;
        private Player player;
        private UniTask recieve;

        private UniTaskCompletionSource<List<Player>> playerListSource;

        public bool Working { get; private set; }


        public event Action<Client> OnStarted;
        public event Action<Client> OnStopped;

        public event Action<Player> OnPlayerConntected;
        public event Action<Player> OnPlayerDisconnected;
        public event Action<Player, string> OnChatMessage;


        public event Action<string, object> OnUpdateObject;

        public event Action<int> OnLoadScene;
        public event Action<Dictionary<string, int>> OnStartRound;
        public event Action<Player> OnEndRound;


        public void SetPlayer(Player player)
        {
            this.player = player;
        }
        public void Start(IPAddress host, int port, bool master)
        {
            serverEndPoint = new IPEndPoint(host, port);
            udpClient = new UdpClient();

            Working = true;

            recieve = UniTask.RunOnThreadPool(RecieveLoop);

            player.Master = master;

            OnStarted?.Invoke(this);
        }
        public async UniTask Stop()
        {
            if (!Working)
                return;
            Working = false;

            await SendMessage(new Message(nameof(Disconnect), player));

            UnityMainThreadDispatcher.Execute(() =>
            {
                Disconnect(player, new RecieveInfo() { EndPoint = serverEndPoint });
            });
            try
            {
                udpClient.Close();
                udpClient = null;
            }
            catch { }

            OnStopped?.Invoke(this);
        }
        public async UniTask Send(string method, params object[] args)
        {
            await SendMessage(new Message(method, args));
        }


        private async UniTask RecieveLoop()
        {
            await SendMessage(new Message(nameof(Connect), player));

            try
            {
                while (Working)
                {
                    await Recieve();
                }
            }
            catch
            {
                await Stop();
            }
        }
        private async UniTask Recieve()
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


            UnityMainThreadDispatcher.Execute(() =>
            {
                Invoker.Invoke(message.MethodName, Concat(message.GetData(), info));
            });
        }
        private async UniTask SendMessage(Message message)
        {
            if (udpClient == null)
                return;

            try
            {
                byte[] data = message.ToBytes();
                await udpClient.SendAsync(data, data.Length, serverEndPoint);
            }
            catch(Exception e)
            {
                Debug.Log($"Client | Exception: {e}");
            }
        }


        //Request Methods
        public async UniTask<List<Player>> GetPlayerList()
        {
            playerListSource = new UniTaskCompletionSource<List<Player>>();

            await Send(nameof(RequestPlayersList));

            return await playerListSource.Task;
        }


        //Network Methods
        public override void Connect(Player player, RecieveInfo info)
        {
            OnPlayerConntected?.Invoke(player);
        }
        public override async void Disconnect(Player player, RecieveInfo info)
        {
            if (player.Server)
            {
                Debug.Log($"Client | Server stopped");
                await Stop();
                return;
            }

            OnPlayerDisconnected?.Invoke(player);

            Debug.Log($"Client | Player: {player.Name} Disconnected.");
        }
        public override void ChatMessage(Player player, string text, RecieveInfo info)
        {
            OnChatMessage?.Invoke(player, text);
        }


        public override void LoadScene(int sceneIndex, RecieveInfo info)
        {
            OnLoadScene?.Invoke(sceneIndex);
        }
        public override void PlayersList(List<Player> players, RecieveInfo info)
        {
            if (playerListSource == null || players == null)
            {
                playerListSource.TrySetException(new Exception("Null reference"));
                return;
            }
            playerListSource.TrySetResult(players);
        }


        public override void StartRound(Dictionary<string, int> score, RecieveInfo info)
        {
            OnStartRound?.Invoke(score);
        }
        public override void EndRound(Player looser, RecieveInfo info)
        {
            OnEndRound?.Invoke(looser);
        }


        public override void UpdateObject(string id, object data, RecieveInfo info)
        {
            OnUpdateObject?.Invoke(id, data);
        }
    }
}
