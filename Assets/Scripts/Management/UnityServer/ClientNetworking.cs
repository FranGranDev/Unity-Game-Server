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

            client.OnLoadScene += CallOnLoadScene;

            client.OnRecieveData += OnRecieveData;

            client.OnUpdateObject += CallOnUpdateObject;
        }

        private void SubscribeToLobby()
        {
            lobby.OnRequestPlayers += GetPlayersList;
        }


        /// <summary>
        /// Use this to start master Client
        /// </summary>
        public void StartMasterClient(int port)
        {
            player.Master = true;

            StartRemoteClient(IPAddress.Parse("127.0.0.1"), port);
        }
        /// <summary>
        /// Use this to start remote Client
        /// </summary>
        public void StartRemoteClient(IPAddress address, int port)
        {
            client = new Client(player, address, port);

            SubscribeToClient();

            client.Start();
        }
        /// <summary>
        /// Stop client work
        /// </summary>
        public void StopClient()
        {
            if (client == null)
                return;

            client.Stop();
            delayedActions.Clear();
        }


        public async void LoadScene(int index)
        {
            Message message = new Message(nameof(client.LoadSceneMessage), index);

            await client.Send(message);
        }
        public async void GetPlayersList(Action<List<Player>> onRecieve)
        {
            Message message = new Message(nameof(client.RequestPlayersList));

            delayedActions.Add(message.Id, onRecieve);

            await client.Send(message);
        }
        public async void UpdateObject(string id, object data)
        {
            Message message = new Message(nameof(client.UpdateObject), id, data);

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

        private void CallOnLoadScene(int index)
        {
            UnityMainThreadDispatcher.Execute(() =>
            {
                OnLoadScene?.Invoke(index);
            });
        }
        private void CallOnUpdateObject(string id, object data)
        {
            UnityMainThreadDispatcher.Execute(() =>
            {
                OnUpdateObject?.Invoke(id, data);
            });
        }
        private void OnRecieveData(object[] data, string id)
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
