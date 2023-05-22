using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class UIClickRotation : UIClickBehavior
    {
        public UIClickRotation(Transform panel, float value, float duration, int vibration, Ease ease) : base(panel, value, duration, vibration, ease)
        {
            startRotation = panel.localRotation;
        }


        private bool isPlaying;
        private Tween tween;
        private Quaternion startRotation;

        public override void Play()
        {
            if (isPlaying)
            {
                return;
            }
            isPlaying = true;
            panel.localRotation = startRotation;
            tween = panel.DOPunchRotation(new Vector3(0, 0, value), duration, vibration).
                SetEase(ease).
                OnComplete(() =>
                {
                    panel.localRotation = startRotation;
                    isPlaying = false;
                });
        }
        public override bool IsPlaying { get => isPlaying; }
    }
}