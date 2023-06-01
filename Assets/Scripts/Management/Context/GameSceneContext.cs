using Game;
using Networking.Data;
using Networking.Server;
using Networking.Messages;
using Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    public class GameSceneContext : SceneContext, IBindable<Client>, IBindable<Server>, IBindable<ILobby>, IGameEvents
    {
        [SerializeField] private GameUI gameUI;
        [SerializeField] private List<PlayerHandler> playerHandlers;
        [SerializeField] private Ball ball;
        [Space]
        [SerializeField] private ObjectSynchronizer objectSynchronizer;


        private Client client;
        private Server server;
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


        public void Bind(Server obj)
        {
            server = obj;
        }
        public void Bind(Client obj)
        {
            client = obj;

            client.OnUpdateObject += UpdateObject;
            client.OnStartRound += StartRound;
            client.OnEndRound += EndRound;
        }
        public void Bind(ILobby obj)
        {
            lobby = obj;
        }
        private void OnDestroy()
        {
            client.OnUpdateObject -= UpdateObject;
            client.OnStartRound -= StartRound;
            client.OnEndRound -= EndRound;
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

        private async void CallStartRound(float delay)
        {
            if (lobby.IsMaster)
            {
                await Task.Delay(Mathf.RoundToInt(delay * 1000));

                await client.Send(nameof(client.StartRound), score);
            }
        }

        private async void OnPlayerLose(Player player)
        {
            if (lobby.IsMaster)
            {
                score[player.Id]++;

                await client.Send(nameof(client.EndRound), player);
            }
        }



        private async void OnObjectUpdated(string id, object data)
        {
            await client.Send(nameof(client.UpdateObject), id, data);
        }
        private void UpdateObject(string id, object data)
        {
            objectSynchronizer.UpdateRemoteObject(id, data);
        }



        private async void Restart()
        {
            await client.Send(nameof(client.LoadScene), 1);
        }
        private void Exit()
        {
            server.Stop();
            client.Stop();

            SceneManager.LoadScene(0);
        }

        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }

    }
}
