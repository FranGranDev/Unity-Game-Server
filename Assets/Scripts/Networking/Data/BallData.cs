using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking.Data
{
    public class BallData
    {
        public BallData()
        {

        }
        public BallData(Rigidbody rigidbody, Vector3 lastVelocity, bool started)
        {
            Rigidbody = new RigidbodyData(rigidbody);
            LastVelocity = new Vector3Data(lastVelocity);
            Started = started;
        }

        public RigidbodyData Rigidbody { get; set; }
        public Vector3Data LastVelocity { get; set; }
        public bool Started { get; set; }
    }
}
