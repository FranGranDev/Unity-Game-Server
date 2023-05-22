using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class UIClickScale : UIClickBehavior
    {
        public UIClickScale(Transform panel, float value, float duration, int vibration, Ease ease) : base(panel, value, duration, vibration, ease)
        {
            localScale = Vector3.one;
        }

        private bool isPlaying;
        private Tween tween;
        private Vector3 localScale;

        public override void Play()
        {
            if (isPlaying)
            {
                return;
            }
            isPlaying = true;
            panel.localScale = localScale;
            tween = panel.DOPunchScale(Vector3.one * value, duration, vibration).
                SetEase(ease).
                OnComplete(() =>
                {
                    isPlaying = false;
                    panel.localScale = localScale;
                });
        }
        public override bool IsPlaying { get => isPlaying; }
    }
}