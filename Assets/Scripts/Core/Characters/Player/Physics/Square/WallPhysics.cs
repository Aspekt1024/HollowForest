using System;
using HollowForest;
using HollowForest.Physics;
using HollowForest.World;
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

        private bool isAttachedToStickySurface;

        public bool IsAttachedToWall { get; private set; }
        public Vector3 WallPosition { get; private set; }

        public WallPhysics(Character character, Settings settings, CollisionSensor collision)
        {
            this.character = character;
            this.settings = settings;
            this.collision = collision;
            
            collision.OnWallHit += OnWallHit;
            
            character.State.RegisterStateObserver(CharacterStates.IsAttachedToWall, OnAttachedToWallStateChanged);
            character.State.RegisterStateObserver(CharacterStates.IsGrappling, OnGrapplingStateChanged);
        }

        private void OnWallHit(Vector3 position, Surface surface)
        {
            if (CanAttachToWall(surface))
            {
                WallPosition = position;
                character.State.SetState(CharacterStates.IsAttachedToWall, true);
            
                character.Effects.OnAttachedToWall(position);
            }
        }

        private void OnAttachedToWallStateChanged(bool isAttachedToWall)
        {
            if (!isAttachedToWall && IsAttachedToWall)
            {
                timeUnattached = Time.time;
                var xDir = WallPosition.x - character.transform.position.x;
                var xVelocity = settings.wallJumpHorizontalVelocity * Mathf.Sign(-xDir);
                character.Physics.SetHorizontalVelocity(xVelocity, settings.wallJumpDuration);
                isAttachedToStickySurface = false;
            }
            
            IsAttachedToWall = isAttachedToWall;
        }

        private void OnGrapplingStateChanged(bool isGrappling)
        {
            if (IsAttachedToWall && !isGrappling && !isAttachedToStickySurface)
            {
                IsAttachedToWall = false;
                timeUnattached = Time.time;
                character.State.SetState(CharacterStates.IsAttachedToWall, false);
            }
        }

        private bool CanAttachToWall(Surface surface)
        {
            if (IsAttachedToWall) return false;
            
            if (surface != null && surface.isSticky)
            {
                isAttachedToStickySurface = true;
                return Time.time >= timeUnattached + settings.wallAttachmentCooldown;
            }
            
            if (!character.Abilities.HasAbility(CharacterAbility.AttachToWall)) return false;

            if (!character.State.GetState(CharacterStates.IsGrappling)) return false;
            if (character.State.GetState(CharacterStates.IsGrounded)) return false;

            return Time.time >= timeUnattached + settings.wallAttachmentCooldown;
        }
    }
}