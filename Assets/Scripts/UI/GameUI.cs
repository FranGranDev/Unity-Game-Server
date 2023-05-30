using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private ButtonUI homeButton;
        [SerializeField] private ButtonUI restartButton;


        public event System.Action OnExit;
        public event System.Action OnRestart;


        public void Initialize(bool master)
        {
            restartButton.gameObject.SetActive(master);

            homeButton.OnClick += OnClickHome;
            restartButton.OnClick += OnClickRestart;
        }

        private void OnClickRestart()
        {
            OnRestart?.Invoke();
        }
        private void OnClickHome()
        {
            OnExit?.Invoke();
        }
    }
}
