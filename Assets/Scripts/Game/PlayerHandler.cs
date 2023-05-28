using Networking.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private Racket racket;
    [SerializeField] private new Camera camera;

    public Player Player { get; private set; }
    public bool Local { get; private set; }

    public void SetPlayer(Player player, bool local)
    {
        Player = player;
        Local = local;

        if(local)
        {
            racket.gameObject.AddComponent<RacketController>();
        }
        else
        {
            camera.gameObject.SetActive(false);
        }
    }
}
