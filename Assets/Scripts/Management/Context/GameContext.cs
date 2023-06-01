using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Services;
using Networking;
using Data;
using Networking.Data;
using Networking.Server;

namespace Management
{
    [DefaultExecutionOrder(-1)]
    public class GameContext : MonoBehaviour, ISceneVisitor
    {
        private static GameContext active;
        private void Awake()
        {
            if(active != null)
            {
                Destroy(gameObject);
                return;
            }

            active = this;
            DontDestroyOnLoad(gameObject);

            SceneContext.OnAwake += OnSceneAwake;


            SetupGame();
            InitializeComponents();
        }

        private Client client;
        private Server server;
        private Lobby lobby;


        private void SetupGame()
        {
            Application.runInBackground = true;
        }
        private void InitializeComponents()
        {
            client = new Client();
            server = new Server();

            lobby = new Lobby(new Player(SavedData.PlayerName));
            lobby.Bind(client);

            client.SetPlayer(lobby.Self);

            client.OnLoadScene += LoadScene;
        }


        private void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }
        private void OnSceneAwake(SceneContext scene)
        {
            scene.Visit(this);
        }

        public void Visited(MenuSceneContext scene)
        {
            InitializeService.Bind(scene, client);
            InitializeService.Bind(scene, server);
            InitializeService.Bind<ILobby>(scene, lobby);
        }

        public void Visited(GameSceneContext scene)
        {
            InitializeService.Bind(scene, client);
            InitializeService.Bind(scene, server);
            InitializeService.Bind<ILobby>(scene, lobby);
        }
    }
}
