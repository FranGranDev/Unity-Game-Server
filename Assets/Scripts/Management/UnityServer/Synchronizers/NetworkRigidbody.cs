using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Data;


namespace Management
{
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkRigidbody : NetworkObject
    {
        private new Rigidbody rigidbody;


        public override object Data
        {
            get
            {
                return new RigidbodyData(rigidbody);
            }
        }


        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public override void Synchronize(object data)
        {
            try
            {
                RigidbodyData rigidbodyData = data as RigidbodyData;

                rigidbody.position = rigidbodyData.Position.GetVector();
                rigidbody.velocity = rigidbodyData.Velocity.GetVector();
                rigidbody.angularVelocity = rigidbodyData.Angular.GetVector();
            }
            catch { Debug.Log($"Cant convert {data} to RigidbodyData", this); }
        }
    }
}
