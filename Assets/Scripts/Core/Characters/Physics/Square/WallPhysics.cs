using System;
using HollowForest;
using HollowForest.Physics;
using UnityEngine;

namespace Core.Characters.Physics.Square
{
    public class WallPhysics
    {
        [Serializable]
        public class Settings
        {
            public float wallAttachmentCooldown = 0.1f;
            public float wallJumpHorizontalVelocity = 3f;
            public float wallJumpDuration = 0.2f;
        }

        private readonly Settings settings;
        private readonly Character character;
        private readonly CollisionSensor collision;

        private float timeUnattached;

        public bool IsAttachedToWall { get; private set; }
        public Vector3 WallPosition { get; private set; }

        public float Direction
        {
            get
            {
                var xDir = WallPosition.x - character.transform.position.x;
                return xDir > 0 ? 1 : -1;
            }
        }

        public WallPhysics(Character character, Settings settings, CollisionSensor collision)
        {
            this.character = character;
            this.settings = settings;
            this.collision = collision;
            
            collision.OnWallHit += OnWallHit;
            
            character.State.RegisterStateObserver(CharacterStates.IsAttachedToWall, OnAttachedToWallStateChanged);
        }

        private void OnWallHit(Vector3 position)
        {
            if (IsAttachedToWall) return;

            if (!character.State.GetState(CharacterStates.IsGrappling)) return;
            if (character.State.GetState(CharacterStates.IsGrounded)) return;
            
            if (Time.time >= timeUnattached + settings.wallAttachmentCooldown)
            {
                WallPosition = position;
                character.State.SetState(CharacterStates.IsAttachedToWall, true);
                
                character.Effects.OnAttachedToWall(position);
            }
        }

        private void OnAttachedToWallStateChanged(bool isAttachedToWall)
        {
            IsAttachedToWall = isAttachedToWall;
            if (!isAttachedToWall)
            {
                timeUnattached = Time.time;
                var xDir = WallPosition.x - character.transform.position.x;
                var xVelocity = settings.wallJumpHorizontalVelocity * Mathf.Sign(-xDir);
                character.Physics.SetHorizontalVelocity(xVelocity, settings.wallJumpDuration);
            }
        }
    }
}