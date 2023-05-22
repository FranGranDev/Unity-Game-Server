using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Services;


namespace UI
{
    public class UIClick : MonoBehaviour, IUIClickBehavior, IBindable<ISoundPlayer>
    {
        [SerializeField] private AnimationTypes animationType;
        [SerializeField] private Transform panel;
        [SerializeField] private float value = 0.33f;
        [SerializeField] private float time = 0.5f;
        [SerializeField] private int vibration = 5;
        [SerializeField] private Ease ease = Ease.InOutSine;
        [Space]
        [SerializeField] private bool playSound;
        [SerializeField] private string soundId;
        [SerializeField] private float soundVolume;
        [SerializeField] private float soundDelay;

        [SerializeField] private bool checkIAddictive = true;

        private IUIClickBehavior behavior;
        private ISoundPlayer soundPlayer;
        private List<IAddictive> loopAnimations = new List<IAddictive>();
        private bool initilized = false;


        public bool IsPlaying
        {
            get => behavior.IsPlaying;
        }


        public void Play()
        {
            if(!initilized)
            {
                Initilize();
            }

            behavior?.Play();

            if (playSound && soundPlayer != null)
            {
                soundPlayer.PlaySound(soundId, soundVolume, 1, soundDelay);
            }
        }
        private void Initilize()
        {
            switch (animationType)
            {
                case AnimationTypes.Rotation:
                    behavior = new UIClickRotation(panel, value, time, vibration, ease);
                    break;
                case AnimationTypes.Scale:
                    behavior = new UIClickScale(panel, value, time, vibration, ease);
                    break;
                case AnimationTypes.None:
                    behavior = null;
                    break;
            }
            if (checkIAddictive)
            {
                loopAnimations = new List<IAddictive>(panel.GetComponentsInChildren<IAddictive>(true));
            }
            initilized = true;
        }
        public void Bind(ISoundPlayer info)
        {
            soundPlayer = info;
        }


        private void Start()
        {
            if(!initilized)
            {
                Initilize();
            }
        }
        private void OnValidate()
        {
            if (panel == null)
            {
                panel = transform;
            }
        }

        public enum AnimationTypes
        {
            None, Scale, Rotation,
        }

        private void Update()
        {
            foreach (IAddictive addictive in loopAnimations)
            {
                addictive.IsPlaying = !IsPlaying;
            }
        }

    }
}