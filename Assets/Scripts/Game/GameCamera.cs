using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Range(0, 1f)] private float ratio;
    [Space]
    [SerializeField] private Transform moveTarget;
    [SerializeField, Range(0, 1f)] private float moveRatio;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        startPosition = transform.localPosition;
        startRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (target)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(direction, Vector3.up), ratio);

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.1f);
        }
        if(moveTarget)
        {
            Vector3 position = Vector3.Lerp(startPosition, startPosition + moveTarget.localPosition, moveRatio);


            transform.localPosition = Vector3.Lerp(transform.localPosition, position, 0.1f);
        }


    }
}
