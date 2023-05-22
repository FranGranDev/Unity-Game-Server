using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Messages.Data
{
    [Serializable]
    public class Player
    {
        public static Player ServerPlayer
        {
            get
            {
                return new Player("Server")
                {
                    Server = true,
                };
            }
        }

        public Player(string name)
        {
            Id = Guid.NewGuid();

            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool Server { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is Player otherPlayer)
            {
                return Id.Equals(otherPlayer.Id);
            }

            return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
