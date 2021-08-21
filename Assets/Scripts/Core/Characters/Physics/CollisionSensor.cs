using System;
using UnityEngine;

namespace HollowForest.Physics
{
    public class CollisionSensor
    {
        private readonly int worldLayer;
        private readonly Vector2 colliderExtents;
        private readonly Character character;

        public event Action OnCeilingHit = delegate { };

        public bool IsGrounded { get; private set; }
        public float TimeUngrounded { get; private set; }

        public CollisionSensor(Character character)
        {
            this.character = character;
            colliderExtents = character.Collider.bounds.extents;
            worldLayer = 1 << LayerMask.NameToLayer("World");
        }

        public Vector3 ValidatePosition(Vector3 pos)
        {
            var currentPos = character.Transform.position;
            var diff = pos - currentPos;

            if (diff.y > 0)
            {
                var hitAbove = Physics2D.Raycast(currentPos, Vector2.up, colliderExtents.y + diff.y, worldLayer);
                if (hitAbove.collider != null)
                {
                    pos.y = hitAbove.point.y - colliderExtents.y;
                    OnCeilingHit?.Invoke();
                }

                var hitBelow = Physics2D.Raycast(pos, Vector2.down, colliderExtents.y + 0.05f, worldLayer);
                if (hitBelow.collider == null)
                {
                    SetGroundedState(false);
                }
            }
            else
            {
                var hitBelow = Physics2D.Raycast(currentPos, Vector2.down, colliderExtents.y - diff.y, worldLayer);
                if (hitBelow.collider != null)
                {
                    pos.y = hitBelow.point.y + colliderExtents.y;
                    SetGroundedState(true);
                }
                else
                {
                    SetGroundedState(false);
                }
            }

            if (diff.x > 0)
            {
                var hitRight = Physics2D.Raycast(currentPos, Vector2.right, colliderExtents.x + diff.x, worldLayer);
                if (hitRight.collider != null)
                {
                    pos.x = hitRight.point.x - colliderExtents.x;
                }
            }
            else
            {
                var hitLeft = Physics2D.Raycast(currentPos, Vector2.left, colliderExtents.x - diff.x, worldLayer);
                if (hitLeft.collider != null)
                {
                    pos.x = hitLeft.point.x + colliderExtents.x;
                }
            }
            
            return pos;
        }

        private void SetGroundedState(bool isOnGround)
        {
            if (!isOnGround && IsGrounded)
            {
                TimeUngrounded = Time.time;
                character.State.SetState(CharacterStates.Grounded, false);
            }
            else if (isOnGround && !IsGrounded)
            {
                character.State.SetState(CharacterStates.Grounded, true);
            }

            IsGrounded = isOnGround;
        }
    }
}