using Networking.ServerSide;
using Networking.ClientSide;
using Networking.Messages.Data;
using UnityEngine;
using System.Net;

namespace Management
{
    public class ServerNetworking : MonoBehaviour
    {
        private Server server;

        public void StartServer(int port)
        {
            if(server != null)
            {
                StopServer();
            }

            server = new Server(port);

            server.Start();
        }

        public void StopServer()
        {
            if (server == null)
                return;
            server.Stop();
            server = null;
        }

        private void OnDestroy()
        {
            StopServer();
        }
    }
}
