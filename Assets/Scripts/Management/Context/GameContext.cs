using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Services;
using Networking;
using Data;
using Networking.Data;

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

        [SerializeField] private ClientNetworking client;
        [SerializeField] private ServerNetworking server;

        public Lobby Lobby { get; private set; }


        private void SetupGame()
        {
            Application.runInBackground = true;
        }
        private void InitializeComponents()
        {
            Lobby = new Lobby(new Player(SavedData.PlayerName));

            client.Initialize(Lobby);

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
            InitializeService.Bind<ILobby>(scene, Lobby);
        }
    }
}
