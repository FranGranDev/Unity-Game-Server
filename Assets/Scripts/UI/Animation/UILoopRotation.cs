using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UILoopRotation : MonoBehaviour, IAddictive
    {
        [SerializeField] private float speed = 1;
        [SerializeField] private bool playAlways = false;

        public bool IsPlaying { private get; set; }

        private void Start()
        {
            IsPlaying = false;
        }
        private void FixedUpdate()
        {
            if (IsPlaying || playAlways)
            {
                transform.Rotate(Vector3.forward, speed * Time.fixedDeltaTime);
            }
        }
    }
}