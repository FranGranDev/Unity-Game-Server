using System.Collections;
using System.Net;
using System.Collections.Generic;
using Networking.Messages.Data;
using UnityEngine;
using UI;
using Services;


namespace Management
{
    public class MenuSceneContext : SceneContext, IBindable<ClientNetworking>, IBindable<ServerNetworking>
    {
        [SerializeField] private MainUI mainUI;

        private ClientNetworking client;
        private ServerNetworking server;


        protected override void Initialize()
        {
            mainUI.OnStartHost += CallStartHost;
            mainUI.OnJoinHost += CallJoinHost;

            InitializeService.Initialize(transform);
        }
        public void Bind(ClientNetworking obj)
        {
            client = obj;

            client.OnPlayerConnected += OnPlayerConnected;
            client.OnPlayerDisconnected += OnPlayerDisconnected;
        }
        public void Bind(ServerNetworking obj)
        {
            server = obj;
        }


        private void CallJoinHost(IPAddress address, int port)
        {
            Debug.Log("Call Join Host");

            client.StartClient(address, port);

        }
        private void CallStartHost(int port)
        {
            Debug.Log("Call Start Host");

            server.StartServer(port);
            this.Delayed(() =>
            {
                client.StartClient(port);
            }, 0.1f);
        }


        private void OnPlayerDisconnected(Player player)
        {

        }
        private void OnPlayerConnected(Player player)
        {
            if (player.Equals(client.Player))
            {
                mainUI.State = MainUI.States.Lobby;
            }
            else
            {
                Debug.Log("Other player connected");
            }
        }


        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
