using UnityEngine;


namespace UI
{
    public class UILoopScale : MonoBehaviour, IAddictive
    {
        [SerializeField] private bool playAlways;
        [SerializeField] private float period = 1f;
        [SerializeField] private float punchScale = 0.1f;

        private float time;

        public bool IsPlaying { get; set; }

        public void Restart()
        {
            IsPlaying = false;
            transform.localScale = Vector3.one;
        }

        private void Start()
        {
            IsPlaying = false;
        }
        private void FixedUpdate()
        {
            if (IsPlaying || playAlways)
            {
                transform.localScale = Vector3.one * (1 + Mathf.Sin(time * period) * punchScale);
                time += Time.fixedDeltaTime;
            }
            else
            {
                time = 0;
            }

        }
    }
}