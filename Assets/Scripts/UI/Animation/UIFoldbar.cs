using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace UI.Animations
{
    public class UIFoldbar : MonoBehaviour
    {
        [Header("Show/Hide Animation Settings")]
        [SerializeField] protected UiAnimationData show;
        [SerializeField] protected UiAnimationData hide;
        [Header("Base Component")]
        [SerializeField] protected Transform panel;

        [Header("Points")]
        [SerializeField] private Transform showPoint;
        [SerializeField] private Transform hidePoint;


        protected List<IUiBehavior> animations = new List<IUiBehavior>();
        private Tween tween;


        public bool IsShown
        {
            get
            {
                return isShown;
            }
            set
            {
                if (isShown == value)
                    return;

                tween?.Kill();

                isShown = value;
                gameObject.SetActive(true);

                foreach (IUiBehavior animation in animations)
                {
                    animation.IsShown = value;
                }
            }
        }

        public bool IsPlaying => false;

        protected bool isShown;


        private void Start()
        {
            AddAnimations();


            UIPanel parent = GetComponentInParent<UIPanel>();
            if(parent)
            {
                parent.OnChangeState += (x) =>
                {
                    IsShown = false;
                };
            }
        }

        public void Click()
        {
            IsShown = !isShown;
        }

        protected void AddAnimations()
        {
            animations.Add(new UIMoveYBehavior(panel, show, hide, showPoint, hidePoint));
            animations.Add(new UiScaleBehavior(panel, show, hide));
        }
    }
}