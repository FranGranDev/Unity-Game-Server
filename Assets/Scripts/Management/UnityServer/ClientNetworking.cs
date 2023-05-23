using Networking.ServerSide;
using Networking.ClientSide;
using Networking.Messages.Data;
using UnityEngine;
using System.Net;

namespace Management
{
    public class ClientNetworking : MonoBehaviour
    {
        private Client client;


        public Player Player { get; set; }


        public event System.Action<Player> OnPlayerConnected;
        public event System.Action<Player> OnPlayerDisconnected;


        public void Initialize()
        {

        }


        /// <summary>
        /// Use this to start local Client
        /// </summary>
        public void StartClient(int port)
        {
            StartClient(IPAddress.Parse("127.0.0.1"), port);
        }
        /// <summary>
        /// Use this to start remote Client
        /// </summary>
        public void StartClient(IPAddress address, int port)
        {
            client = new Client(Player, address, port);


            client.OnPlayerConntected += CallOnPlayerConnected;
            client.OnPlayerDisconnected += CallOnPlayerDisconnected;


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
        }



        private void CallOnPlayerConnected(Player player)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                OnPlayerConnected?.Invoke(player);
            });
        }

        private void CallOnPlayerDisconnected(Player player)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                OnPlayerDisconnected?.Invoke(player);
            });
        }



        private void OnDestroy()
        {
            StopClient();
        }
    }
}
