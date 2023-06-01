using System.Collections;
using System.Net;
using System.Collections.Generic;
using Networking.Data;
using UnityEngine;
using UI;
using Services;
using System.Threading.Tasks;
using Networking.Server;
using Networking.Messages;


namespace Management
{
    public class MenuSceneContext : SceneContext, IBindable<Client>, IBindable<Server>
    {
        [SerializeField] private MainUI mainUI;

        private Client client;
        private Server server;
        private ILobby lobby;

        protected override void Initialize()
        {
            mainUI.OnStartHost += CallStartHost;
            mainUI.OnJoinHost += CallJoinHost;

            mainUI.OnLeaveLobby += CallLeaveLobby;
            mainUI.OnStartGame += CallStartGame;

            InitializeService.Initialize(transform);
        }


        public void Bind(Client obj)
        {
            client = obj;
        }
        public void Bind(Server obj)
        {
            server = obj;
        }

        private void CallJoinHost(IPAddress address, int port)
        {
            client.Start(address, port, false);
        }
        private void CallStartHost(int port)
        {
            server.Start(port);
            client.Start(IPAddress.Parse("127.0.0.1"), port, true);
        }


        private async void CallStartGame()
        {
            await client.Send(nameof(client.LoadScene), 1);
        }
        private async void CallLeaveLobby()
        {
            await client.Stop();
            await server.Stop();
        }


        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }

    }
}
