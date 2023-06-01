using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using NaughtyAttributes;
using Networking.Data;
using Services;
using Data;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace UI
{
    public class MainUI : MonoBehaviour, Initializable, IBindable<ILobby>
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
        [Foldout("Lobby"), SerializeField] private ButtonUI startGameButton;
        [Foldout("Lobby"), SerializeField] private UIPanel startGamePanel;
        [Foldout("Lobby"), SerializeField] private ButtonUI leaveLobbyButton;
        [Foldout("Lobby"), SerializeField] private PlayerList lobbyPlayers;

        [Foldout("States"), SerializeField] private States state;

        private Dictionary<States, IEnumerable<UIPanel>> menuPanels;

        private ILobby lobby;

        public States State
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


        public event System.Action<int> OnStartHost;
        public event System.Action<IPAddress, int> OnJoinHost;


        public event System.Action OnStartGame;
        public event System.Action OnLeaveLobby;
        

        public async void Initialize()
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


            inputName.Fill(SavedData.PlayerName);
            inputName.OnFilled += ChangePlayerName;

            hostPortNumber.Fill(SavedData.HostPort.ToString());
            hostPortNumber.OnFilled += ChangePortNumber;

            string address = $"{SavedData.JoinAddress}:{SavedData.JoinPort}";
            joinIPAddress.Fill(address);
            joinIPAddress.OnFilled += ChangeJoinAddress;

            hostButton.OnClick += GoHostMenu;
            joinButton.OnClick += GoJoinMenu;
            hostBackButton.OnClick += GoMainMenu;
            joinBackButton.OnClick += GoMainMenu;

            hostLobbyButton.OnClick += StartHost;
            joinLobbyButton.OnClick += JoinHost;

            startGameButton.OnClick += StartGame;
            leaveLobbyButton.OnClick += LeaveLobby;

            await UniTask.Delay(100);

            TurnUI(true);
            State = States.Main;
        }

        public void Bind(ILobby lobby)
        {
            this.lobby = lobby;

            lobby.OnConnected += OnConnected;
            lobby.OnDisconnected += OnDisconnected;
            lobby.OnOtherConnected += OnOtherConnected;
            lobby.OnOtherDisconnected += OnOtherDisconnected;
        }


        private void OnDestroy()
        {
            if (lobby != null)
            {
                lobby.OnConnected -= OnConnected;
                lobby.OnDisconnected -= OnDisconnected;
                lobby.OnOtherConnected -= OnOtherConnected;
                lobby.OnOtherDisconnected -= OnOtherDisconnected;
            }
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

            lobby.Self.Name = name;
        }
        private void ChangePortNumber(string name)
        {
            int port = 0;
            if(int.TryParse(name, out port))
            {
                SavedData.HostPort = port;
            }
            else
            {
                hostPortNumber.Fill("Invalid Value!");
            }
        }
        private void ChangeJoinAddress(string text)
        {
            var address = text.Split(":");

            if(address.Length != 2)
            {
                joinIPAddress.Fill("Invalid Value!");
                return;
            }

            if(IPAddress.TryParse(address[0], out IPAddress ip) && int.TryParse(address[1], out int port))
            {
                SavedData.JoinAddress = address[0];
                SavedData.JoinPort = port;
            }
            else
            {
                joinIPAddress.Fill("Invalid Value!");
            }

        }
        private void UpdateStartButton()
        {
            startGamePanel.IsShown = lobby.Self.Master && lobby.Players.Count > 1;
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


        private void StartHost()
        {
            OnStartHost?.Invoke(SavedData.HostPort);
        }
        private void JoinHost()
        {
            IPAddress address = IPAddress.Parse(SavedData.JoinAddress);

            OnJoinHost?.Invoke(address, SavedData.JoinPort);
        }


        private void StartGame()
        {
            OnStartGame?.Invoke();
        }
        private void LeaveLobby()
        {
            OnLeaveLobby?.Invoke();
        }


        private void OnOtherDisconnected(Player player)
        {
            lobbyPlayers.RemovePlayer(player);

            UpdateStartButton();
        }
        private void OnOtherConnected(Player player)
        {
            lobbyPlayers.Clear();
            lobbyPlayers.AddPlayers(lobby.Players);

            UpdateStartButton();
        }

        private void OnDisconnected(Player obj)
        {
            State = States.Main;
        }
        private void OnConnected(Player player)
        {
            State = States.Lobby;
            lobbyPlayers.Clear();

            lobbyPlayers.AddPlayers(lobby.Players);

            UpdateStartButton();
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