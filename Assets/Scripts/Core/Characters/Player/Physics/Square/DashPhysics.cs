using System;
using HollowForest.World;
using UnityEngine;

namespace HollowForest.Physics
{
    public class DashPhysics
    {
        [Serializable]
        public class Settings
        {
            public float distance = 5f;
            public float speed = 30f;
            public float cooldown = 0.6f;
        }

        private readonly Settings settings;
        private readonly CollisionSensor collision;
        private readonly Character character;

        private bool isDashActive;
        private float timeStartedDash;
        private float timeDashEnds;
        private bool isDirectionRight;
        
        private bool isGroundedSinceLastDash;

        public bool IsDashing => isDashActive;

        public DashPhysics(Character character, Settings settings, CollisionSensor collision)
        {
            this.character = character;
            this.settings = settings;
            this.collision = collision;

            isGroundedSinceLastDash = true;
            isDashActive = false;
            timeStartedDash = -1000;
            
            // TODO consider using this for early dash leway: character.State.RegisterStateObserver(CharacterStates.IsRecovering, OnRecoveryStateChanged);
            collision.OnWallHit += OnWallHit;
            character.State.RegisterStateObserver(CharacterStates.IsGrounded, OnGroundedStateChanged);
            
            character.Abilities.EnableAbility(CharacterAbility.Dash);
        }

        public Vector3 CalculateVelocity(Vector3 velocity)
        {
            if (!isDashActive) return velocity;
            velocity.x = settings.speed * (isDirectionRight ? 1f : -1f);
            
            var remainingTime = timeDashEnds - Time.time;
            if (remainingTime < 0f)
            {
                var overshoot = remainingTime * (isDirectionRight ? settings.speed : -settings.speed);
                velocity.x -= overshoot / Time.fixedDeltaTime;
                CancelDash();
            }
            
            // TODO hangtime if in air at end of dash
            
            return velocity;
        }

        public float CalculateHeight(Vector3 velocity, Vector3 pos)
        {
            // TODO omnidirectional dash
            return pos.y;
        }

        public void DashRequested()
        {
            if (!CanDash()) return;
            
            timeStartedDash = Time.time;
            isDashActive = true;
            if (!character.State.GetState(CharacterStates.IsGrounded))
            {
                isGroundedSinceLastDash = false;
            }
            timeDashEnds = timeStartedDash + settings.distance / settings.speed;
            isDirectionRight = character.State.GetState(CharacterStates.IsFacingRight);
            character.State.SetState(CharacterStates.IsDashing, true);
            character.Effects.BeginDash();
        }

        public void CancelDash()
        {
            isDashActive = false;
            character.State.SetState(CharacterStates.IsDashing, false);
            character.Effects.EndDash();
        }

        private void OnWallHit(Vector3 wallPoint, Surface surface)
        {
            if (isDashActive)
            {
                CancelDash();
            }
        }

        private bool CanDash()
        {
            if (!character.Abilities.HasAbility(CharacterAbility.Dash)) return false;
            return isGroundedSinceLastDash && !isDashActive && Time.time >= timeStartedDash + settings.cooldown;
        }

        private void OnGroundedStateChanged(bool isGrounded)
        {
            if (isGrounded) isGroundedSinceLastDash = true;
        }
    }
}