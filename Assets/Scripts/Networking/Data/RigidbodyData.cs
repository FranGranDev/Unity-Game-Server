using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking.Data
{
    public class RigidbodyData
    {
        public RigidbodyData()
        {

        }
        public RigidbodyData(Rigidbody rigidbody)
        {
            Position = new Vector3Data(rigidbody.position);
            Velocity = new Vector3Data(rigidbody.velocity);
            Angular = new Vector3Data(rigidbody.angularVelocity);
        }

        public void Accept(Rigidbody rigidbody)
        {
            rigidbody.position = Position.GetVector();
            rigidbody.velocity = Velocity.GetVector();
            rigidbody.angularVelocity = Angular.GetVector();
        }


        public Vector3Data Position { get; set; }
        public Vector3Data Velocity { get; set; }
        public Vector3Data Angular { get; set; }
    }
}
