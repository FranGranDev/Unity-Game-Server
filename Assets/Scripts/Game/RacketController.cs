using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Racket))]
public class RacketController : MonoBehaviour
{
    private Racket racket;

    private void Awake()
    {
        racket = GetComponent<Racket>();
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            racket.Move(-Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            racket.Move(Time.deltaTime);
        }
    }
}
