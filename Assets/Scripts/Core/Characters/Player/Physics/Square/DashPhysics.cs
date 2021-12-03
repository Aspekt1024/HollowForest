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
            public float distance = 4f;
            public float speed = 20f;
            public float cooldown = 0.8f;
        }

        private readonly Settings settings;
        private readonly CollisionSensor collision;
        private readonly Character character;

        private bool isDashActive;
        private float timeStartedDash;
        private float startPosX;
        private bool isDirectionRight;
        private bool canDash;

        public bool IsDashing => isDashActive;

        public DashPhysics(Character character, Settings settings, CollisionSensor collision)
        {
            this.character = character;
            this.settings = settings;
            this.collision = collision;

            canDash = true;
            isDashActive = false;
            timeStartedDash = -1000;
            
            // TODO consider using this for early dash leway: character.State.RegisterStateObserver(CharacterStates.IsRecovering, OnRecoveryStateChanged);
            collision.OnWallHit += OnWallHit;
        }

        public Vector3 CalculateVelocity(Vector3 velocity)
        {
            if (!isDashActive) return velocity;
            velocity.x = settings.speed * (isDirectionRight ? 1f : -1f);
            
            var overshoot = character.transform.position.x + velocity.x * Time.fixedDeltaTime - startPosX - settings.distance;
            if (overshoot > 0f) // TODO account for negative direction when dashing left
            {
                velocity.x -= overshoot / Time.fixedDeltaTime;
                isDashActive = false;
            }
            
            // TODO hangtime if in air at end of dash
            
            return velocity;
        }

        public float CalculateHeight(Vector3 velocity, Vector3 pos)
        {
            return pos.y;
        }

        public void DashRequested()
        {
            if (!canDash || Time.time < timeStartedDash + settings.cooldown) return;
            
            timeStartedDash = Time.time;
            isDashActive = true;
            startPosX = character.transform.position.x;
            // TODO which direction is the character facing?
            isDirectionRight = true;
        }

        private void OnWallHit(Vector3 wallPoint, Surface surface)
        {
            if (isDashActive)
            {
                isDashActive = false;
            }
        }
    }
}