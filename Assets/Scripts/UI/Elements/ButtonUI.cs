using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class ButtonUI : MonoBehaviour
    {
        [SerializeField] private string id; 
        [Space, SerializeField] private UIClick clickAnimation;

        private Button button;

        public string ID => id;
        public event System.Action OnClick;



        private void Awake()
        {
            button = GetComponentInChildren<Button>(true);
            button.onClick.AddListener(Click);
        }

        public void Click()
        {
            if (!button.IsInteractable())
                return;
            clickAnimation?.Play();
            OnClick?.Invoke();
        }
    }
}
