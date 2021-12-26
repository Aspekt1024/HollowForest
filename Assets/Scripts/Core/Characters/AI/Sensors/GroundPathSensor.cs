
using System.Collections.Generic;
using UnityEngine;

namespace HollowForest.AI
{
    public class GroundPathSensor : AISensor
    {
        public Collider2D leftGroundCollider;
        public Collider2D rightGroundCollider;

        private readonly List<Collider2D> colliders = new List<Collider2D>();
        
        protected override void OnInit()
        {
            
        }

        private void Update()
        {
            var numContacts = leftGroundCollider.GetContacts(colliders);
            agent.memory.SetState(AIState.IsNearLeftEdge, numContacts == 0);
            
            numContacts = rightGroundCollider.GetContacts(colliders);
            agent.memory.SetState(AIState.IsNearRightEdge, numContacts == 0);
        }
    }
}