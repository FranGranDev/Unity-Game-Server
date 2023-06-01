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


        [NetworkMethod(nameof(Connect))]
        public virtual void Connect(Player player, RecieveInfo info)
        {

        }


        [NetworkMethod(nameof(Disconnect))]
        public virtual void Disconnect(Player player, RecieveInfo info)
        {

        }


        [NetworkMethod(nameof(LoadScene))]
        public virtual void LoadScene(int sceneIndex, RecieveInfo info)
        {

        }


        [NetworkMethod(nameof(RequestPlayersList))]
        public virtual void RequestPlayersList(RecieveInfo info)
        {

        }

        [NetworkMethod(nameof(PlayersList))]
        public virtual void PlayersList(List<Player> players, RecieveInfo info)
        {

        }


        [NetworkMethod(nameof(StartRound))]
        public virtual void StartRound(Dictionary<string, int> score, RecieveInfo info)
        {

        }

        [NetworkMethod(nameof(EndRound))]
        public virtual void EndRound(Player winner, RecieveInfo info)
        {

        }



        [NetworkMethod(nameof(UpdateObject))]
        public virtual void UpdateObject(string id, object data, RecieveInfo info)
        {

        }
    }
}
