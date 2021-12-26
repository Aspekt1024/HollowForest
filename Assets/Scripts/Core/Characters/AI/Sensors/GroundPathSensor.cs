
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
            var hasLeftWall = leftWallCollider.GetContacts(colliders) > 0;
            var isLeftPathObstructed = !hasLeftGround || hasLeftWall;
            agent.memory.SetState(AIState.IsNearLeftEdge, isLeftPathObstructed);
            
            var hasRightGround = rightGroundCollider.GetContacts(colliders) > 0;
            var hasRighttWall = rightWallCollider.GetContacts(colliders) > 0;
            var isRightPathObstructed = !hasRightGround || hasRighttWall;
            agent.memory.SetState(AIState.IsNearRightEdge, isRightPathObstructed);
        }
    }
}