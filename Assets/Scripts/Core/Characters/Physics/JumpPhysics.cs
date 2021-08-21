using System;
using UnityEngine;

namespace HollowForest.Physics
{
    public class JumpPhysics
    {

        [Serializable]
        public class Settings
        {
            public float maxJumpHeight = 4f;
            public AnimationCurve jumpCurve;
            
            public float minAirTime;
            public float maxAirTime;

            [Tooltip("The time allowance for when jump is pressed before hitting the ground")]
            public float earlyJumpLeway;
            
            [Tooltip("The time allowance for when jump was pressed after leaving the ground")]
            public float lateJumpLeway;
        }

        private readonly Settings settings;
        private readonly CollisionSensor collision;
        private readonly Character character;

        private bool isJumpHeld;
        private bool isJumpActive;
        private float timeStartedJumping;
        private float initialHeight;
        private float timeJumpRequested;
        private bool canJump;

        public bool IsJumping => isJumpActive;
        
        public JumpPhysics(Character character, Settings settings, CollisionSensor collision)
        {
            this.character = character;
            this.settings = settings;
            this.collision = collision;

            canJump = true;
            timeStartedJumping = -1000;
            
            character.State.RegisterStateObserver(CharacterStates.Grounded, OnGroundStateChanged);
            character.State.RegisterStateObserver(CharacterStates.IsRecovering, OnRecoveryStateChanged);
            collision.OnCeilingHit += OnCeilingHit;
        }

        public void JumpRequested()
        {
            isJumpHeld = true;
            if (canJump && (collision.IsGrounded || Time.time < collision.TimeUngrounded + settings.lateJumpLeway))
            {
                BeginJump();
            }
            else
            {
                timeJumpRequested = Time.time;
            }
        }

        public void JumpReleased()
        {
            isJumpHeld = false;
        }

        public float CalculateHeight()
        {
            if (isJumpActive && Time.time > timeStartedJumping + settings.maxAirTime)
            {
                isJumpActive = false;
                character.State.SetState(CharacterStates.Jumping, false);
                return character.Transform.position.y;
            }

            if (isJumpActive)
            {
                if (isJumpHeld || Time.time < timeStartedJumping + settings.minAirTime)
                {
                    var normalizedJumpTime = (Time.time - timeStartedJumping) / settings.maxAirTime;
                    return initialHeight + settings.maxJumpHeight * settings.jumpCurve.Evaluate(normalizedJumpTime);
                }
            }
            
            isJumpActive = false;
            character.State.SetState(CharacterStates.Jumping, false);
            return character.Transform.position.y;
        }

        private void BeginJump()
        {
            if (isJumpActive) return;
            
            isJumpActive = true;
            character.State.SetState(CharacterStates.Jumping, true);
            timeStartedJumping = Time.time;
            initialHeight = character.Transform.position.y;
        }

        private void OnGroundStateChanged(bool isGrounded)
        {
            if (isGrounded)
            {
                CheckEarlyJumpLeway();
            }
        }

        private void OnRecoveryStateChanged(bool isRecovering)
        {
            canJump = !isRecovering;
            CheckEarlyJumpLeway();
        }

        private void CheckEarlyJumpLeway()
        {
            if (canJump && Time.time < timeJumpRequested + settings.earlyJumpLeway)
            {
                BeginJump();
            }
        }

        private void OnCeilingHit()
        {
            if (isJumpActive)
            {
                isJumpActive = false;
                character.State.SetState(CharacterStates.Jumping, false);
            }
        }
    }
}