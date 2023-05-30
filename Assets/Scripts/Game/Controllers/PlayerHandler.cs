using Networking.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;


namespace Game
{
    public class PlayerHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Racket racket;
        [SerializeField] private new Camera camera;
        [SerializeField] private TextMeshPro score;
        [Space]
        [SerializeField] private Trigger loseTrigger;

        public Player Player { get; private set; }
        public bool Local { get; private set; }


        public event System.Action<Player> OnLose;


        private void Awake()
        {
            loseTrigger.OnEnter += OnBallEnter;
        }


        public void SetPlayer(Player player, bool local)
        {
            Player = player;
            Local = local;


            camera.gameObject.SetActive(local);
            if(local)
            {
                racket.gameObject.AddComponent<RacketController>();
            }
            else
            {
                score.gameObject.SetActive(false);
            }
        }
        public void UpdateScore(Dictionary<string, int> data)
        {
            string otherPlayer = data.Keys.First(x => !x.Equals(Player.Id));

            score.text = $"{data[otherPlayer]}:{data[Player.Id]}";
        }


        private void OnBallEnter(Collider obj)
        {
            if (!obj.GetComponentInParent<Ball>())
                return;

            OnLose?.Invoke(Player);
        }
    }
}