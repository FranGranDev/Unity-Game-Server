using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class PlayerListElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private Image playerIcon;
        [Space]
        [SerializeField] private Sprite masterSprite;
        [SerializeField] private Sprite playerSprite;

        public string Id { get; private set; }

        public void Initialize(string id, string name, bool master)
        {
            Id = id;

            playerName.text = name;
            playerIcon.sprite = master ? masterSprite : playerSprite;
        }
    }
}
