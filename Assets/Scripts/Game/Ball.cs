using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;


namespace Game
{
    public class Ball : MonoBehaviour, IBindable<IGameEvents>
    {
        [Header("Settings")]
        [SerializeField] private float startSpeed;
        [SerializeField] private float hitSpeedIncrease;
        [Header("Wall Reflect Settings")]
        [Range(0, 1f), SerializeField] private float wallReflectRandomize;
        [Range(1, 2f), SerializeField] private float wallReflectZMultiple;
        [Header("Racket Reflect Settings")]
        [Range(0, 1f), SerializeField] private float racketReflectRandomize;
        [Range(0, 1f), SerializeField] private float racketReflectRatio;
        [Header("Components")]
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private GameObject model;
        [SerializeField] private TrailRenderer trailRenderer;

        public float CurrantSpeed { get; set; }
        public bool Started { get; set; }
        public Vector3 LastVelocity { get; set; }
        public Rigidbody Rigidbody => rigidbody;
        public Color TrailColor
        {
            set
            {
                trailRenderer.startColor = new Color(value.r, value.g, value.b, 0.75f);
            }
        }


        public void Bind(IGameEvents obj)
        {
            obj.OnStart += StartGame;
            obj.OnEnd += EndGame;
        }

        private void StartGame()
        {
            CurrantSpeed = startSpeed;
            transform.localPosition = Vector3.zero;

            Started = true;
            trailRenderer.emitting = true;
            model.SetActive(true);

            rigidbody.velocity = Random.onUnitSphere * startSpeed;
        }
        private void EndGame()
        {
            Started = false;
            trailRenderer.emitting = false;
            model.SetActive(false);

            rigidbody.velocity = Vector3.zero;
            transform.localPosition = Vector3.zero;
        }


        private void WallReflect(Vector3 normal)
        {
            Vector3 newDirection = Vector3.Reflect(LastVelocity.normalized, normal).normalized;

            newDirection = new Vector3(newDirection.x, newDirection.y, newDirection.z * wallReflectZMultiple).normalized;
            newDirection = (newDirection + Random.onUnitSphere * wallReflectRandomize).normalized;

            rigidbody.velocity = newDirection * CurrantSpeed;
        }
        private void RacketReflect(Racket racket, Vector3 normal)
        {
            CurrantSpeed += hitSpeedIncrease;

            Vector3 reflectDirection = Vector3.Reflect(LastVelocity.normalized, normal).normalized;
            Vector3 offsetDirection = (transform.position - racket.transform.position).normalized;

            Vector3 newDirection = Vector3.Lerp(reflectDirection, offsetDirection, racketReflectRatio);

            newDirection = (newDirection + Random.onUnitSphere * racketReflectRandomize).normalized;

            rigidbody.velocity = newDirection * CurrantSpeed;

            TrailColor = racket.Color;
        }

        private void Update()
        {
            if (!Started)
                return;

            LastVelocity = rigidbody.velocity;
        }
        private void FixedUpdate()
        {
            if (!Started)
                return;
            rigidbody.velocity = rigidbody.velocity.normalized * CurrantSpeed;
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (!Started)
                return;

            RacketModel model = collision.transform.GetComponentInChildren<RacketModel>();
            if (model != null)
            {
                RacketReflect(model.Racket, collision.GetContact(0).normal);
                return;
            }

            WallReflect(collision.GetContact(0).normal);
        }

    }
}