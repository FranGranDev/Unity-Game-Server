using System.Collections;
using System.Net;
using System.Collections.Generic;
using Networking.Data;
using UnityEngine;
using UI;
using Services;
using System.Threading.Tasks;

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

            mainUI.OnLeaveLobby += CallLeaveLobby;
            mainUI.OnStartGame += CallStartGame;

            InitializeService.Initialize(transform);
        }


        public void Bind(ClientNetworking obj)
        {
            client = obj;
        }
        public void Bind(ServerNetworking obj)
        {
            server = obj;
        }


        private void CallJoinHost(IPAddress address, int port)
        {
            client.StartRemoteClient(address, port);
        }
        private void CallStartHost(int port)
        {
            server.StartServer(port);
            client.StartMasterClient(port);
        }


        private void CallStartGame()
        {
            client.LoadScene(1);
        }
        private void CallLeaveLobby()
        {
            server.StopServer();
            client.StopClient();
        }


        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
