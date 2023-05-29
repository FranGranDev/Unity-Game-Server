using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Racket))]
public class RacketController : MonoBehaviour
{
    [SerializeField] private InputType inputType;

    private Racket racket;

    private void Awake()
    {
        racket = GetComponent<Racket>();
    }


    private void Update()
    {
        switch(inputType)
        {
            case InputType.WASD:
                if (Input.GetKey(KeyCode.A))
                {
                    racket.Move(-Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    racket.Move(Time.deltaTime);
                }
                break;
            case InputType.IJKL:
                if (Input.GetKey(KeyCode.L))
                {
                    racket.Move(-Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.J))
                {
                    racket.Move(Time.deltaTime);
                }
                break;
        }

    }

    private enum InputType
    {
        WASD,
        IJKL,
    }
}
