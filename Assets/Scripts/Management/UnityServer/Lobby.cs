using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Data;
using System.Linq;
using Services;
using Networking.Server;
using Cysharp.Threading.Tasks;

namespace Management
{
    public class Lobby : ILobby, IBindable<Client>
    {
        public Lobby(Player self)
        {
            Self = self;

            Players = new List<Player>()
            {
                Self,
            };
        }

        private Client client;

        public bool IsMaster
        {
            get
            {
                return Self.Master;
            }
        }
        public Player Self { get; set; }
        public List<Player> Players { get; set; }


        public event Action<Player> OnConnected;
        public event Action<Player> OnOtherConnected;

        public event Action<Player> OnDisconnected;
        public event Action<Player> OnOtherDisconnected;

        public event Action<Player, string> OnChatMessage;


        public void Bind(Client client)
        {
            this.client = client;

            client.OnPlayerConntected += OnPlayerConnected;
            client.OnPlayerDisconnected += OnPlayerDisconnected;
            client.OnChatMessage += OnPlayerChatMessage;

            client.OnStopped += (x) => Restart();
        }
        public void Restart()
        {
            Players = new List<Player>()
            {
                Self,
            };
        }


        public async void OnPlayerConnected(Player player)
        {
            if(Self.Equals(player))
            {
                if(!Self.Master)
                {
                    Players = await client.GetPlayerList();
                    OrderPlayers();
                }

                OnConnected?.Invoke(player);
            }
            else
            {
                AddPlayer(player);

                OrderPlayers();

                OnOtherConnected?.Invoke(player);
            }
        }
        public void OnPlayerDisconnected(Player player)
        {
            if (Self.Equals(player))
            {
                OnDisconnected?.Invoke(player);
            }
            else
            {
                RemovePlayer(player);

                OrderPlayers();

                OnOtherDisconnected?.Invoke(player);
            }
        }

        public void OnPlayerChatMessage(Player player, string text)
        {
            if (Self.Equals(player))
                return;

            OnChatMessage?.Invoke(player, text);
        }


        private void AddPlayer(Player player)
        {
            if(Players.FirstOrDefault(x => x.Equals(player)) == null)
            {
                Players.Add(player);

                OrderPlayers();
            }
        }
        private void RemovePlayer(Player player)
        {
            if (Players.FirstOrDefault(x => x.Equals(player)) != null)
            {
                Players.Remove(player);

                OrderPlayers();
            }
        }

        private void OrderPlayers()
        {
            Players = Players
                .OrderBy(x => x.Id)
                .ToList();

            Players
                .ForEach(x => x.Index = Players.IndexOf(x));
        }
    }
}
