
using System.Collections.Generic;
using UnityEngine;

namespace HollowForest.AI
{
    public class GroundPathSensor : AISensor
    {
        public Collider2D leftGroundCollider;
        public Collider2D rightGroundCollider;
        public Collider2D leftWallCollider;
        public Collider2D rightWallCollider;

        private readonly List<Collider2D> colliders = new List<Collider2D>();

        protected override void OnInit()
        {
            
        }

        private void Update()
        {
            var hasLeftGround = leftGroundCollider.GetContacts(colliders) > 0;
            agent.memory.SetState(AIState.IsNearLeftEdge, !hasLeftGround);
            
            var hasLeftWall = leftWallCollider.GetContacts(colliders) > 0;
            agent.memory.SetState(AIState.IsNearLeftWall, hasLeftWall);
            
            var hasRightGround = rightGroundCollider.GetContacts(colliders) > 0;
            agent.memory.SetState(AIState.IsNearRightEdge, !hasRightGround);
            
            var hasRighttWall = rightWallCollider.GetContacts(colliders) > 0;
            agent.memory.SetState(AIState.IsNearRightWall, hasRighttWall);
        }
    }
}