using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Networking;
using Data;
using Networking.Messages.Data;

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


        private void SetupGame()
        {
            Application.runInBackground = true;
        }
        private void InitializeComponents()
        {
            client.Initialize();

            client.Player = new Player(SavedData.PlayerName);
        }


        private void OnSceneAwake(SceneContext scene)
        {
            scene.Visit(this);
        }

        public void Visited(MenuSceneContext scene)
        {
            InitializeService.Bind(scene, client);
            InitializeService.Bind(scene, server);

        }
    }
}
