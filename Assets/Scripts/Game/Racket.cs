using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racket : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed;
    [Header("Components")]
    [SerializeField] private Rigidbody racket;
    [SerializeField] private MeshRenderer meshRenderer;
    [Space]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;


    public float PositionX { get; set; } = 0;
    public Color Color { get => meshRenderer.sharedMaterial.color; }


    public void Move(float delta)
    {
        PositionX += delta * speed;

        PositionX = Mathf.Clamp(PositionX, leftPoint.localPosition.x, rightPoint.localPosition.x);
    }


    private void FixedUpdate()
    {
        Vector3 position = racket.transform.localPosition;
        position.x = PositionX;

        racket.transform.localPosition = position;
    }
}
