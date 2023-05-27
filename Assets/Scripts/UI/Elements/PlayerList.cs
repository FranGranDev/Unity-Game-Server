using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Data;
using System.Linq;

namespace UI
{
    public class PlayerList : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [Space]
        [SerializeField] private PlayerListElement elementPrefab;

        private List<PlayerListElement> players = new List<PlayerListElement>();

        public void AddPlayer(Player player)
        {
            PlayerListElement element = Instantiate(elementPrefab, container);
            element.Initialize(player.Id, player.Name, player.Master);

            players.Add(element);
        }
        public void AddPlayers(List<Player> data)
        {
            foreach (Player player in data)
            {
                if (players.FirstOrDefault(x => x.Id == player.Id) != null)
                    continue;

                PlayerListElement element = Instantiate(elementPrefab, container);
                element.Initialize(player.Id, player.Name, player.Master);

                players.Add(element);
            }
        }
        public void RemovePlayer(Player player)
        {
            PlayerListElement element = players.FirstOrDefault(x => x.Id == player.Id);
            if(element)
            {
                players.Remove(element);
                Destroy(element.gameObject);
            }
        }

        public void Clear()
        {
            foreach(PlayerListElement player in players)
            {
                Destroy(player.gameObject);
            }

            players.Clear();
        }
    }
}
