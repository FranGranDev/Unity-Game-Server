using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class Trigger : MonoBehaviour
{
    [SerializeField] private LayerMask mask;

    public event System.Action<Collider> OnEnter;


    private void OnTriggerEnter(Collider other)
    {
        if (mask != (mask | (1 << other.gameObject.layer)))
            return;

        OnEnter?.Invoke(other);
    }
}
