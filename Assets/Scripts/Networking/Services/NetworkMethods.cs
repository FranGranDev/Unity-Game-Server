using Networking.Attributes;
using Networking.Messages.Data;
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
        public virtual void ErrorMessage(string error)
        {
            SafeDebugger.Log(error);
        }


        [NetworkMethod(nameof(ChatMessage))]
        public virtual void ChatMessage(Player player, string text, IPEndPoint endPoint)
        {
            SafeDebugger.Log($"{player.Name}: {text}");
        }


        [NetworkMethod(nameof(ConnectMessage))]
        public virtual void ConnectMessage(Player player, IPEndPoint endPoint)
        {

        }


        [NetworkMethod(nameof(DisconnectMessage))]
        public virtual void DisconnectMessage(Player player, IPEndPoint endPoint)
        {

        }
    }
}
