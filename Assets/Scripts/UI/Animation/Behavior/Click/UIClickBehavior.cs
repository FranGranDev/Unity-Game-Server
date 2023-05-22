using UnityEngine;
using DG.Tweening;

namespace UI
{
    public abstract class UIClickBehavior : IUIClickBehavior
    {
        protected Transform panel;
        protected float value;
        protected float duration;
        protected int vibration;
        protected Ease ease;

        public UIClickBehavior(Transform panel, float value, float duration, int vibration, Ease ease)
        {
            this.panel = panel;
            this.value = value;
            this.vibration = vibration;
            this.ease = ease;
            this.duration = duration;
        }


        public abstract void Play();
        public abstract bool IsPlaying { get; }
    }
}