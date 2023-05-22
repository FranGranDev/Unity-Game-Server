using UnityEngine;
using DG.Tweening;

public class UiScaleBehavior : UiBehavior
{
    public UiScaleBehavior(Transform panel, UiAnimationData showData, UiAnimationData hideData) : base(panel, showData, hideData)
    {

    }

    public override void Hide()
    {
        if (tween.IsActive())
        {
            tween.Kill();
        }

        IsPlaying = true;
        tween = panel.DOScale(0, hideData.time).
            SetDelay(hideData.delay + Time.fixedDeltaTime).
            SetEase(hideData.ease).
            SetUpdate(true).
            OnKill(() => {
                IsPlaying = false;
                panel.gameObject.SetActive(false);
            });
    }

    public override void Show()
    {
        if (tween.IsActive())
        {
            tween.Kill();
        }

        panel.localScale = Vector3.zero;
        IsPlaying = true;
        tween = panel.DOScale(1, showData.time).
            SetDelay(showData.delay + Time.fixedDeltaTime).
            SetEase(showData.ease).
            SetUpdate(true).
            OnStart(() => {
                panel.gameObject.SetActive(true); 
            }).
            OnKill(() => IsPlaying = false);
    }
}
