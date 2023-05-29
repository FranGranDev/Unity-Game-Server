using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RacketModel : MonoBehaviour
    {
        [SerializeField] private Racket racket;

        public Racket Racket { get => racket; }
    }
}
