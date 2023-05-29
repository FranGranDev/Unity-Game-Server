using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Data;

namespace Management 
{
    public abstract class NetworkObject : MonoBehaviour
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public bool Mine { get; set; }
        public abstract object Data { get; }



        public void SetId(Player player)
        {
            Id = $"{player.Id}_{name}";
        }
        
        public abstract void Synchronize(object data);
    }
}
