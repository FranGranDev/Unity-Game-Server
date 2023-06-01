using Cysharp.Threading.Tasks;
using Networking.Data;
using System.Collections.Generic;

public interface ILobby
{
    public bool IsMaster { get; }
    public Player Self { get; set; }
    public List<Player> Players { get; set; }


    public event System.Action<Player> OnConnected;
    public event System.Action<Player> OnOtherConnected;

    public event System.Action<Player> OnDisconnected;
    public event System.Action<Player> OnOtherDisconnected;

    public event System.Action<Player, string> OnChatMessage;
}
