using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;


namespace Management
{
    public abstract class SceneContext : MonoBehaviour, ISceneContext
    {

        public static event System.Action<SceneContext> OnAwake;

        public GameComponents Components { get; set; }

        public Transform Transform
        {
            get => transform;
        }


        private void Awake()
        {
            OnAwake?.Invoke(this);

            Initialize();
        }

        protected abstract void Initialize();

        public abstract void Visit(ISceneVisitor visitor);
    }
}
