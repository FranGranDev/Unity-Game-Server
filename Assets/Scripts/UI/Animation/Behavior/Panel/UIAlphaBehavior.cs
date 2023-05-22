using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIAlphaBehavior : UiBehavior
{
    public UIAlphaBehavior(Transform panel, CanvasGroup canvas, UiAnimationData showData, UiAnimationData hideData) : base(panel, showData, hideData)
    {
        this.canvas = canvas;
        if (canvas)
        {
            canvas.alpha = 0;
        }
    }

    private CanvasGroup canvas;
    private Coroutine coroutine;

    public override void Hide()
    {        
        if (tween.IsActive())
        {
            tween.Kill();
        }

        IsPlaying = true;
        tween = DOTween.To(() => canvas.alpha, (x) => canvas.alpha = x, 0, hideData.time).
            SetDelay(hideData.delay).
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

        if (canvas.alpha > 0.99f)
        {
            canvas.alpha = 0;
        }

        IsPlaying = true;
        panel.gameObject.SetActive(true);
        tween = DOTween.To(() => canvas.alpha, (x) => canvas.alpha = x, 1, showData.time).
            SetDelay(showData.delay).
            SetEase(showData.ease).
            SetUpdate(true).
            OnKill(() => {
                IsPlaying = false;
            });
    }
}