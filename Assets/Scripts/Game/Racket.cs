using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racket : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed;
    [Header("Components")]
    [SerializeField] private Rigidbody racket;
    [Space]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;


    private float positionX = 0;

    public Rigidbody Rigidbody => racket;


    public void Move(float delta)
    {
        positionX += delta * speed;

        positionX = Mathf.Clamp(positionX, leftPoint.localPosition.x, rightPoint.localPosition.x);
    }


    private void FixedUpdate()
    {
        Vector3 position = racket.transform.localPosition;
        position.x = positionX;

        racket.transform.localPosition = position;
    }
}
