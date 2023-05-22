using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Services;
using Data;

namespace UI
{
    public class MainUI : MonoBehaviour
    {
        [Foldout("Main"), SerializeField] private Transform mainCanvas;
        [Foldout("Main"), SerializeField] private InputUI inputName;
        [Foldout("Main"), SerializeField] private ButtonUI hostButton;
        [Foldout("Main"), SerializeField] private ButtonUI joinButton;
        

        [Foldout("Host"), SerializeField] private Transform hostCanvas;
        [Foldout("Host"), SerializeField] private InputUI hostPortNumber;
        [Foldout("Host"), SerializeField] private ButtonUI hostLobbyButton;
        [Foldout("Host"), SerializeField] private ButtonUI hostBackButton;

        [Foldout("Join"), SerializeField] private Transform joinCanvas;
        [Foldout("Join"), SerializeField] private InputUI joinIPAddress;
        [Foldout("Join"), SerializeField] private ButtonUI joinLobbyButton;
        [Foldout("Join"), SerializeField] private ButtonUI joinBackButton;

        [Foldout("Lobby"), SerializeField] private Transform lobbyCanvas; 

        [Foldout("States"), SerializeField] private States state;

        private Dictionary<States, IEnumerable<UIPanel>> menuPanels;

        private States State
        {
            get => state;
            set
            {
                if (!menuPanels.ContainsKey(value))
                {
                    state = value;
                    return;
                }

                OnStateEnd(state);
                state = value;
                OnStateStart(state);
            }
        }


        private void Start()
        {
            Initialize();
        }
        public void Initialize()
        {
            TurnUI(false);

            menuPanels = new Dictionary<States, IEnumerable<UIPanel>>()
            {
                {States.Host, hostCanvas.GetComponentsInChildren<UIPanel>(true) },
                {States.Main, mainCanvas.GetComponentsInChildren<UIPanel>(true) },
                {States.Join, joinCanvas.GetComponentsInChildren<UIPanel>(true) },
                {States.Lobby, lobbyCanvas.GetComponentsInChildren<UIPanel>(true) },
                {States.None, new List<UIPanel>() },
            };

            GetComponentsInChildren<UIPanel>(true)
                .ToList()
                .ForEach(x => x.Initilize());

            InitializeService.Initialize(transform);


            inputName.Fill(SavedData.PlayerName);
            inputName.OnFilled += ChangePlayerName;

            hostButton.OnClick += GoHostMenu;
            joinButton.OnClick += GoJoinMenu;
            hostBackButton.OnClick += GoMainMenu;
            joinBackButton.OnClick += GoMainMenu;


            this.Delayed(() =>
            {
                TurnUI(true);
                State = States.Main;
            }, 0.1f);
        }

        private void OnStateEnd(States state)
        {
            foreach (UIPanel panel in menuPanels[state])
            {
                panel.IsShown = false;
            }
        }
        private void OnStateStart(States state)
        {
            foreach (UIPanel panel in menuPanels[state])
            {
                panel.IsShown = true;
            }
        }
        private void TurnUI(bool value)
        {
            mainCanvas.gameObject.SetActive(value);
            hostCanvas.gameObject.SetActive(value);
            joinCanvas.gameObject.SetActive(value);
            lobbyCanvas.gameObject.SetActive(value);
        }


        private void ChangePlayerName(string name)
        {
            SavedData.PlayerName = name;
        }


        private void GoMainMenu()
        {
            State = States.Main;
        }
        private void GoJoinMenu()
        {
            State = States.Join;
        }
        private void GoHostMenu()
        {
            State = States.Host;
        }


        public enum States
        {
            None,
            Main,
            Host,
            Join,
            Lobby,
        }
    }
}