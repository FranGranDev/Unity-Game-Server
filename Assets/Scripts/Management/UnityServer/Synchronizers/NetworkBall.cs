using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Data;
using Game;


namespace Management
{
    [RequireComponent(typeof(Ball))]
    public class NetworkBall : NetworkObject
    {
        private Ball ball;

        public override object Data
        {
            get => new BallData(ball.Rigidbody, ball.LastVelocity, ball.Started);
        }

        private void Awake()
        {
            ball = GetComponent<Ball>();    
        }



        public override void Synchronize(object data)
        {
            try
            {
                BallData ballData = (BallData)data;

                ballData.Rigidbody.Accept(ball.Rigidbody);
                ball.LastVelocity = ballData.LastVelocity.GetVector();
                ball.Started = ballData.Started;
            }
            catch { Debug.Log($"Cant convert {data} to BallData", gameObject); }
        }
    }
}
