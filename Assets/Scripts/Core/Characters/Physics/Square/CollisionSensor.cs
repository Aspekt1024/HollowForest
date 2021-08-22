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
        public float CurrentGradient { get; private set; }
        public Vector3 CurrentGroundPoint { get; private set; }

        public CollisionSensor(Character character)
        {
            this.character = character;
            colliderExtents = character.Collider.bounds.extents;
            worldLayer = 1 << LayerMask.NameToLayer("World");
        }

        public Vector3 ProcessBounds(Vector3 pos)
        {
            var currentPos = character.Transform.position;
            var diff = pos - currentPos;
            
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

            var groundHit = CalculateGroundHit(pos, diff);
            CurrentGradient = groundHit.gradient;
            if (groundHit.isGrounded)
            {
                pos.y = groundHit.height + colliderExtents.y;
                CurrentGroundPoint = groundHit.groundPoint;
            }
            SetGroundedState(groundHit.isGrounded);

            if (diff.y > 0)
            {
                var hitAbove = Physics2D.Raycast(pos, Vector2.up, colliderExtents.y + diff.y, worldLayer);
                if (hitAbove.collider != null)
                {
                    pos.y = hitAbove.point.y - colliderExtents.y;
                    OnCeilingHit?.Invoke();
                }
            }
            
            return pos;
        }

        private struct GroundHit
        {
            public bool isGrounded;
            public float height;
            public float gradient;
            public Vector2 groundPoint;
        }

        private GroundHit CalculateGroundHit(Vector3 currentPos, Vector3 diff)
        {
            var pos = currentPos;
            var raycastDist = colliderExtents.y;
            pos.x += diff.x;
            
            if (diff.y > 0)
            {
                pos.y += diff.y;
                raycastDist += 0.5f;
            }
            else
            {
                raycastDist -= diff.y;
            }
            
            var hitCentre = Physics2D.Raycast(pos, Vector2.down, raycastDist, worldLayer);
            
            var leftPos = pos + Vector3.left * (colliderExtents.x - 0.05f);
            var rightPos = pos + Vector3.right * (colliderExtents.x - 0.05f);
            var slopeDetectionDist = 1f;
            var hitLeft = Physics2D.Raycast(leftPos, Vector2.down, raycastDist + slopeDetectionDist, worldLayer);
            var hitRight = Physics2D.Raycast(rightPos, Vector2.down, raycastDist + slopeDetectionDist, worldLayer);

            var groundDetectedCentre = hitCentre.collider != null;
            var groundDetectedLeft = hitLeft.collider != null;
            var groundDetectedRight = hitRight.collider != null;
            var groundedLeft = groundDetectedLeft && hitLeft.point.y >= pos.y - raycastDist - 0.05f;
            var groundedRight = groundDetectedRight && hitRight.point.y >= pos.y - raycastDist - 0.05f;
            
            var isGrounded = groundDetectedCentre || groundedLeft || groundedRight;
            if (!isGrounded)
            {
                return new GroundHit { isGrounded = false };
            }

            var height = 0f;
            if (groundDetectedCentre)
            {
                height = hitCentre.point.y;
            }
            else if (groundDetectedLeft)
            {
                height = hitLeft.point.y;
            }
            else
            {
                height = hitRight.point.y;
            }
            
            var gradient = groundDetectedCentre ? 0f : 1f;
            if (groundDetectedLeft && groundDetectedRight)
            {
                var lrDiff = hitRight.point - hitLeft.point;
                //gradient = lrDiff.y / lrDiff.x;
            }

            var groundPoint = pos;
            groundPoint.y = height;
            
            var details = new GroundHit
            {
                isGrounded = Mathf.Abs(gradient) < 0.5f,
                height = height,
                gradient = gradient,
                groundPoint = groundPoint,
            };

            return details;
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