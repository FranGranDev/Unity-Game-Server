using Networking.Messages;
using Networking.Messages.Data;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Networking.ServerSide
{
    public class Handler
    {
        public Handler(UdpClient server, IPEndPoint endPoint, Player player)
        {
            Server = server;
            EndPoint = endPoint;
            Player = player;
        }

        public Player Player { get; }
        public IPEndPoint EndPoint { get; }
        public UdpClient Server { get; }


        public async void Send(Message message)
        {
            byte[] data = message.ToBytes();
            await Server.SendAsync(data, data.Length, EndPoint);
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
