using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Data;

namespace Management 
{
    public abstract class NetworkObject : MonoBehaviour
    {
        public string Id { get; private set; } = "null";
        public bool Mine { get; set; }

        public event System.Action<string, object> OnUpdated;


        public void SetId(Player player)
        {
            Id = $"{player.Id}_{name}";
        }

        protected void CallUpdate(object data)
        {
            OnUpdated?.Invoke(Id, data);
        }
        public abstract void Synchronize(object data);
    }
}
