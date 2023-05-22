using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class UIToggle : MonoBehaviour
    {
        [Header("Toggle Settings")]
        [SerializeField] private Image icon;
        [SerializeField] private Sprite spriteOn;
        [SerializeField] private Sprite spriteOff;

        [SerializeField] private UIClick onAnimation;
        [SerializeField] private UIClick offAnimation;

        private System.Action<bool> onClick;
        private bool value;

        public void Initialize(System.Action<bool> onTurnSound, bool value)
        {
            onClick = onTurnSound;
            this.value = value;

            SetIcon();
        }

        public void OnClick()
        {
            value = !value;

            onClick?.Invoke(value);

            SetIcon();
            PlayAnimation();
        }

        private void SetIcon()
        {
            Sprite sprite = value ? spriteOn : spriteOff;
            icon.sprite = sprite;
        }
        private void PlayAnimation()
        {
            if (value)
            {
                onAnimation.Play();
            }
            else
            {
                offAnimation.Play();
            }
        }
    }
}