using Networking.Attributes;
using Networking.Messages;
using Networking.Data;
using System.Collections.Generic;
using System.Net;


namespace Networking.Services
{
    public class NetworkMethods
    {
        public NetworkMethods()
        {
            Invoker = new NetworkMethodInvoker(this);
        }

        public NetworkMethodInvoker Invoker { get; }


        public object[] Concat(object[] args, object arg)
        {
            object[] data = new object[args.Length + 1];

            args.CopyTo(data, 0);
            data[data.Length - 1] = arg;

            return data;
        }



        [NetworkMethod(nameof(ErrorMessage))]
        public virtual void ErrorMessage(string error, RecieveInfo info)
        {
            SafeDebugger.Log(error);
        }


        [NetworkMethod(nameof(ChatMessage))]
        public virtual void ChatMessage(Player player, string text, RecieveInfo info)
        {
            SafeDebugger.Log($"{player.Name}: {text}");
        }


        [NetworkMethod(nameof(ConnectMessage))]
        public virtual void ConnectMessage(Player player, RecieveInfo info)
        {

        }


        [NetworkMethod(nameof(DisconnectMessage))]
        public virtual void DisconnectMessage(Player player, RecieveInfo info)
        {

        }


        [NetworkMethod(nameof(LoadSceneMessage))]
        public virtual void LoadSceneMessage(int sceneIndex, RecieveInfo info)
        {

        }


        [NetworkMethod(nameof(RequestPlayersList))]
        public virtual void RequestPlayersList(RecieveInfo info)
        {

        }
        [NetworkMethod(nameof(PlayersListMessage))]
        public virtual void PlayersListMessage(List<Player> players, RecieveInfo info)
        {

        }
    }
}
