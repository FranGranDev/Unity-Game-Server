using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace UI
{
    public class UIModifyPanel : UIPanel
    {
        [Header("Animation")]
        [SerializeField] private List<UiAnimationType> uiAnimations;
        [Header("Points")]
        [SerializeField] private Transform showPoint;
        [SerializeField] private Transform hidePoint;

        protected override void AddAnimations()
        {

            uiAnimationTypes = new Dictionary<UiAnimationType, IUiBehavior>()
        {
            {UiAnimationType.Move, new UiMoveBehavior(panel, show, hide, showPoint, hidePoint) },
            {UiAnimationType.Scale, new UiScaleBehavior(panel, show, hide) },
            {UiAnimationType.Alpha, new UIAlphaBehavior(panel, panel.GetComponent<CanvasGroup>(), show, hide) },
        };

            foreach (UiAnimationType type in uiAnimations)
            {
                animations.Add(uiAnimationTypes[type]);
            }
        }

        private Dictionary<UiAnimationType, IUiBehavior> uiAnimationTypes;

        private enum UiAnimationType
        {
            Move,
            Scale,
            Alpha,
        }


        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (showPoint && hidePoint && Selection.activeGameObject == gameObject)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(hidePoint.position, showPoint.position);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hidePoint.position, 15);
                Gizmos.DrawSphere(showPoint.position, 15);
            }

            if (uiAnimations != null && panel)
            {
                foreach (UiAnimationType type in uiAnimations)
                {
                    if (type == UiAnimationType.Alpha && panel.GetComponent<CanvasGroup>() == null)
                    {
                        uiAnimations.Remove(type);
                        Debug.LogWarning("Panel: " + panel.name + " do not contains CanvasGroup");
                        break;
                    }
                }
            }
#endif
        }
    }
}