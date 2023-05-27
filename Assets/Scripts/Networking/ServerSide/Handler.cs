using Networking.Data;
using Networking.Messages;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Networking.ServerSide
{
    public class Handler
    {
        public Handler(UdpClient server, IPEndPoint endPoint)
        {
            Server = server;
            EndPoint = endPoint;
        }

        public Player Player { get; set; }
        public IPEndPoint EndPoint { get; }
        public UdpClient Server { get; }


        public async Task Send(Message message)
        {
            if (Server == null)
                return;
            
            try
            {
                byte[] data = message.ToBytes();
                await Server.SendAsync(data, data.Length, EndPoint);
            }
            catch { }
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
