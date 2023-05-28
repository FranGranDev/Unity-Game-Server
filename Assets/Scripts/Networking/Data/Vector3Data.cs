using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking.Data
{
    public class Vector3Data
    {
        public Vector3Data(Vector3 vector)
        {
            Coordinate = new float[3]
            {
                vector.x,
                vector.y,
                vector.z,
            };
        }

        public float[] Coordinate { get; set; }

        public Vector3 GetVector()
        {
            return new Vector3(Coordinate[0], Coordinate[1], Coordinate[2]);
        }
    }
}
