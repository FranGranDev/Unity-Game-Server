using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management
{
    [RequireComponent(typeof(Racket))]
    public class NetworkRacket : NetworkObject
    {
        private Racket racket;

        public override object Data
        {
            get => racket.PositionX;
        }

        private void Awake()
        {
            racket = GetComponent<Racket>();
        }

        public override void Synchronize(object data)
        {
            if(float.TryParse(data.ToString(), out float value))
            {
                racket.PositionX = value;
            }
        }
    }
}
