using System.Collections;
using System.Linq;
using Networking.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Management
{
    public class GameSceneContext : SceneContext, IBindable<ClientNetworking>, IBindable<ILobby>
    {
        [SerializeField] private List<PlayerHandler> playerHandlers;
        [SerializeField] private ObjectSynchronizer objectSynchronizer;


        private ClientNetworking client;
        private ILobby lobby;


        protected override void Initialize()
        {
            foreach (Player player in lobby.Players)
            {
                bool local = player.Equals(lobby.Self);

                PlayerHandler handler = playerHandlers[player.Index];

                handler.SetPlayer(player, local);

                handler.GetComponentsInChildren<NetworkObject>()
                    .ToList()
                    .ForEach(x =>
                    {
                        x.SetId(player);
                        x.Mine = local;
                    });
            }

            objectSynchronizer.Initialize();

            objectSynchronizer.OnObjectUpdated += OnObjectUpdated;
        }


        public void Bind(ClientNetworking obj)
        {
            client = obj;

            client.OnUpdateObject += UpdateObject;
        }

        private void OnObjectUpdated(string id, object data)
        {
            client.UpdateObject(id, data);
        }
        private void UpdateObject(string id, object data)
        {
            objectSynchronizer.UpdateRemoteObject(id, data);
        }

        public void Bind(ILobby obj)
        {
            lobby = obj;
        }


        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
