using UnityEngine;
using DG.Tweening;

public abstract class UiBehavior : IUiBehavior
{
    public UiBehavior(Transform panel, UiAnimationData showData, UiAnimationData hideData)
    {
        this.showData = showData;
        this.hideData = hideData;
        this.panel = panel;
    }

    protected UiAnimationData showData;
    protected UiAnimationData hideData;

    protected Transform panel;
    protected Tween tween;

    public bool IsShown
    {
        get
        {
            return isShown;
        }
        set
        {
            if (panel == null)
                return;
            if (value)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
    private bool isShown;

    public virtual void Initilize(bool startState = false)
    {

    }

    public bool IsPlaying { get; protected set; }
    public abstract void Show();
    public abstract void Hide();
}

[System.Serializable]
public sealed class UiAnimationData
{
    public float time = 0.5f;
    public float delay = 0f;
    public Ease ease = Ease.InOutSine;
}