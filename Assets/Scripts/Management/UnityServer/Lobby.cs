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

            Others = new List<Player>();
        }

        public bool IsMaster
        {
            get
            {
                return Self.Master;
            }
        }
        public Player Self { get; set; }
        public List<Player> Others { get; set; }


        public event Action<Player> OnConnected;
        public event Action<Player> OnOtherConnected;

        public event Action<Player> OnDisconnected;
        public event Action<Player> OnOtherDisconnected;

        public event Action<Player, string> OnChatMessage;

        public event Action<Action<List<Player>>> OnRequestPlayers;


        public void OnPlayerConnected(Player player)
        {
            if(Self.Equals(player))
            {
                OnConnected?.Invoke(player);
            }
            else
            {
                AddPlayer(player);

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
            if(Others.FirstOrDefault(x => x.Equals(player)) == null)
            {
                Others.Add(player);
            }
        }
        private void RemovePlayer(Player player)
        {
            if (Others.FirstOrDefault(x => x.Equals(player)) != null)
            {
                Others.Remove(player);
            }
        }


        public void RequestPlayers(Action<List<Player>> onGet)
        {
            OnRequestPlayers?.Invoke(onGet);
        }
    }
}
