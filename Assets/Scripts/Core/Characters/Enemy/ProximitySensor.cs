using System;
using UnityEngine;

namespace HollowForest
{
    public class ProximitySensor : MonoBehaviour
    {
        private Enemy owner;
        
        public void InitAwake(Enemy owner)
        {
            this.owner = owner;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var character = other.GetComponent<Character>();
            if (character != null)
            {
                owner.ThreatDetected(character);
            }
        }
    }
}