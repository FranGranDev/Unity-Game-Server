using Networking.Messages;
using System;
using Networking.ClientSide;
using Networking.Data;
using UnityEngine;
using System.Net;
using System.Collections.Generic;

namespace Management
{
    public class ClientNetworking : MonoBehaviour
    {
        private Client client;
        private Lobby lobby;
        private Player player;

        private Dictionary<string, Delegate> delayedActions;

        public event Action<Dictionary<string, int>> OnRoundStarted;
        public event Action<Player> OnRoundEnded;
        public event Action<string, object> OnUpdateObject;
        public event Action<int> OnLoadScene; 


        public void Initialize(Lobby lobby)
        {
            this.lobby = lobby;

            SubscribeToLobby();

            player = lobby.Self;
            delayedActions = new Dictionary<string, Delegate>();
        }
        private void SubscribeToClient()
        {
            client.OnPlayerConntected += CallPlayerConnected;
            client.OnPlayerDisconnected += CallPlayerDisconnected;
            client.OnChatMessage += CallChatMessage;

            client.OnLoadScene += CallLoadScene;
            client.OnEndRound += CallRoundEnded;
            client.OnStartRound += CallRoundStarted;

            client.OnRecieveData += CallDataRecieved;
            client.OnUpdateObject += CallUpdateObject;
        }
        private void SubscribeToLobby()
        {
            lobby.OnRequestPlayers += GetPlayersList;
        }


        public void StartMasterClient(int port)
        {
            player.Master = true;

            StartRemoteClient(IPAddress.Parse("127.0.0.1"), port);
        }
        public void StartRemoteClient(IPAddress address, int port)
        {
            client = new Client(player, address, port);

            SubscribeToClient();

            client.Start();
        }
        public void StopClient()
        {
            if (client == null)
                return;

            client.Stop();
            delayedActions.Clear();
            lobby.Restart();
        }


        public async void LoadScene(int index)
        {
            if (client == null)
                return;
            Message message = new Message(nameof(client.LoadSceneMessage), index);

            await client.Send(message);
        }
        public async void GetPlayersList(Action<List<Player>> onRecieve)
        {
            if (client == null)
                return;
            Message message = new Message(nameof(client.RequestPlayersList));

            delayedActions.Add(message.Id, onRecieve);

            await client.Send(message);
        }
        public async void UpdateObject(string id, object data)
        {
            if (client == null)
                return;
            Message message = new Message(nameof(client.UpdateObject), id, data);

            await client.Send(message);
        }

        public async void StartRound(Dictionary<string, int> score)
        {
            if (client == null)
                return;
            Message message = new Message(nameof(client.StartRoundMessage), score);

            await client.Send(message);
        }
        public async void EndRound(Player player)
        {
            if (client == null)
                return;
            Message message = new Message(nameof(client.EndRoundMessage), player);

            await client.Send(message);
        }



        private void CallPlayerConnected(Player player)
        {
            UnityMainThreadDispatcher.Execute(() =>
            {
                lobby.OnPlayerConnected(player);
            });
        }
        private void CallPlayerDisconnected(Player player)
        {
            UnityMainThreadDispatcher.Execute(() =>
            {
                lobby.OnPlayerDisconnected(player);
            });
        }
        private void CallChatMessage(Player player, string text)
        {
            UnityMainThreadDispatcher.Execute(() =>
            {
                lobby.OnPlayerChatMessage(player, text);
            });
        }

        private void CallLoadScene(int index)
        {
            UnityMainThreadDispatcher.Execute(() =>
            {
                OnLoadScene?.Invoke(index);
            });
        }
        private void CallRoundStarted(Dictionary<string, int> score)
        {
            UnityMainThreadDispatcher.Execute(() =>
            {
                OnRoundStarted?.Invoke(score);
            });
        }
        private void CallRoundEnded(Player looser)
        {
            UnityMainThreadDispatcher.Execute(() =>
            {
                OnRoundEnded?.Invoke(looser);
            });
        }


        private void CallUpdateObject(string id, object data)
        {
            UnityMainThreadDispatcher.Execute(() =>
            {
                OnUpdateObject?.Invoke(id, data);
            });
        }
        private void CallDataRecieved(object[] data, string id)
        {
            if(delayedActions.ContainsKey(id))
            {
                UnityMainThreadDispatcher.Execute(() =>
                {
                    delayedActions[id]?.DynamicInvoke(data);
                    delayedActions.Remove(id);
                });
            }
        }


        private void OnDestroy()
        {
            StopClient();
        }
    }
}
