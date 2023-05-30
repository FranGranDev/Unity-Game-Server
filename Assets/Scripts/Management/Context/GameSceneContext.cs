using System.Collections;
using System.Linq;
using Networking.Data;
using System.Collections.Generic;
using UnityEngine;
using Services;
using UI;
using Game;
using UnityEngine.SceneManagement;

namespace Management
{
    public class GameSceneContext : SceneContext, IBindable<ClientNetworking>, IBindable<ServerNetworking>, IBindable<ILobby>, IGameEvents
    {
        [SerializeField] private GameUI gameUI;
        [SerializeField] private List<PlayerHandler> playerHandlers;
        [SerializeField] private Ball ball;
        [Space]
        [SerializeField] private ObjectSynchronizer objectSynchronizer;


        private ClientNetworking client;
        private ServerNetworking server;
        private ILobby lobby;

        private Dictionary<string, int> score;


        public event System.Action OnStart;
        public event System.Action OnEnd;


        protected override void Initialize()
        {
            InitializeService.Bind<IGameEvents>(this, this);

            gameUI.Initialize(lobby.IsMaster);

            SetupSynchronize();

            CallStartRound(1f);

            gameUI.OnExit += Exit;
            gameUI.OnRestart += Restart;
        }
        private void SetupSynchronize()
        {
            score = new Dictionary<string, int>();

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

                handler.OnLose += OnPlayerLose;

                score.Add(player.Id, 0);
            }

            Player master = lobby.Players.FirstOrDefault(x => x.Master);
            if (master != null)
            {
                ball.GetComponentsInChildren<NetworkObject>()
                    .ToList()
                    .ForEach(x =>
                    {
                        x.SetId(master);
                        x.Mine = master.Equals(lobby.Self);
                    });
            }

            objectSynchronizer.OnObjectUpdated += OnObjectUpdated;
            objectSynchronizer.Initialize();
        }


        public void Bind(ServerNetworking obj)
        {
            server = obj;
        }
        public void Bind(ClientNetworking obj)
        {
            client = obj;

            client.OnUpdateObject += UpdateObject;
            client.OnRoundStarted += StartRound;
            client.OnRoundEnded += EndRound;
        }
        public void Bind(ILobby obj)
        {
            lobby = obj;
        }
        private void OnDestroy()
        {
            client.OnUpdateObject -= UpdateObject;
            client.OnRoundStarted -= StartRound;
            client.OnRoundEnded -= EndRound;
        }


        private void StartRound(Dictionary<string, int> score)
        {
            this.score = score;

            playerHandlers.ForEach(x => x.UpdateScore(score));

            OnStart?.Invoke();
        }
        private void EndRound(Player looser)
        {
            OnEnd?.Invoke();

            CallStartRound(1f);
        }

        private void CallStartRound(float delay)
        {
            if (lobby.IsMaster)
            {
                this.Delayed(() =>
                {
                    client.StartRound(score);
                }, delay);
            }
        }

        private void OnPlayerLose(Player player)
        {
            if (lobby.IsMaster)
            {
                score[player.Id]++;

                client.EndRound(player);
            }
        }



        private void OnObjectUpdated(string id, object data)
        {
            client.UpdateObject(id, data);
        }
        private void UpdateObject(string id, object data)
        {
            objectSynchronizer.UpdateRemoteObject(id, data);
        }



        private void Restart()
        {
            client.LoadScene(1);
        }
        private void Exit()
        {
            server.StopServer();
            client.StopClient();

            SceneManager.LoadScene(0);
        }

        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }

    }
}
