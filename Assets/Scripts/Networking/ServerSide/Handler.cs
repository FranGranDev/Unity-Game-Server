using Networking.Messages;
using Networking.Messages.Data;
using System.Net;
using System.Net.Sockets;

namespace Networking.ServerSide
{
    public class Handler
    {
        public Handler(Socket server, IPEndPoint endPoint, Player player)
        {
            Server = server;
            EndPoint = endPoint;
            Player = player;
        }

        public Player Player { get; }
        public IPEndPoint EndPoint { get; }
        public Socket Server { get; }


        public async void Send(Message message)
        {
            await Server.SendToAsync(message.ToBytes(), SocketFlags.None, EndPoint);
        }




        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Handler otherHandler = (Handler)obj;
            return EndPoint.Equals(otherHandler.EndPoint);
        }
        public override int GetHashCode()
        {
            return EndPoint.GetHashCode();
        }
    }
}
