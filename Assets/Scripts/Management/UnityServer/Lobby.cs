using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Data;
using System.Linq;


namespace Management
{
    public class Lobby : ILobby
    {
        public Lobby(Player self)
        {
            Self = self;

            Players = new List<Player>()
            {
                Self,
            };
        }

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

        public event Action<List<Player>> OnGetPlayers;

        public event Action<Action<List<Player>>> OnRequestPlayers;


        public void Restart()
        {
            Players = new List<Player>()
            {
                Self,
            };
        }

        public void OnPlayerConnected(Player player)
        {
            if(Self.Equals(player))
            {
                OnConnected?.Invoke(player);

                if(!Self.Master)
                {
                    OnRequestPlayers?.Invoke(GetPlayers);
                }
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


        public void GetPlayers(List<Player> players)
        {
            Players = players;

            OrderPlayers();

            OnGetPlayers?.Invoke(Players);
        }
    }
}
