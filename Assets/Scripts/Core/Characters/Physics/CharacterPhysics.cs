using System;
using HollowForest.Physics;
using UnityEngine;

namespace HollowForest
{
    public class CharacterPhysics
    {
        [Serializable]
        public class Settings
        {
            [Header("Horizontal Movement")]
            public float moveSpeed = 10f;
            
            [Header("Vertical Movement")]
            public float gravity = -60f;
            public float maxFallSpeed = -50f;

            public JumpPhysics.Settings jumpSettings;
        }

        private enum HorizontalInput
        {
            None,
            Left,
            Right,
        }

        private HorizontalInput horizontal;
        private bool canMove;
        
        private Vector2 velocity;
        private float fallStartHeight;

        private readonly Character character;
        private readonly Settings settings;

        private readonly JumpPhysics jump;
        public CollisionSensor Collision { get; }

        public CharacterPhysics(Character character, Settings settings)
        {
            this.character = character;
            this.settings = settings;

            Collision = new CollisionSensor(character);
            jump = new JumpPhysics(character, settings.jumpSettings, Collision);

            character.State.RegisterStateObserver(CharacterStates.Jumping, OnJumpStateChanged);
            character.State.RegisterStateObserver(CharacterStates.Grounded, OnGroundedStateChanged);
            character.State.RegisterStateObserver(CharacterStates.IsRecovering, OnRecoverStateChanged);

            // Set initial conditions
            SetOnGround();
            canMove = true;
        }

        private void SetOnGround()
        {
            var extent = character.Collider.bounds.extents.y;
            var pos = character.Transform.position;
            var hit = Physics2D.Raycast(pos, Vector2.down, 10f, 1 << LayerMask.NameToLayer("World"));
            if (hit.collider != null)
            {
                pos.y = hit.point.y + extent;
                character.Transform.position = pos;
            }
        }

        public void Tick_Fixed()
        {
            var pos = character.Transform.position;
            
            // TODO lerp movement speed
            if (canMove)
            {
                switch (horizontal)
                {
                    case HorizontalInput.None:
                        velocity.x = 0;
                        break;
                    case HorizontalInput.Left:
                        velocity.x = -settings.moveSpeed;
                        break;
                    case HorizontalInput.Right:
                        velocity.x = settings.moveSpeed;
                        break;
                    default:
                        velocity.x = 0;
                        break;
                }
            }
            else
            {
                velocity.x = 0;
            }

            pos.x += velocity.x * Time.fixedDeltaTime;
            
            if (jump.IsJumping)
            {
                pos.y = jump.CalculateHeight();
            }
            else
            {
                if (velocity.y > 0) velocity.y = 0;
                velocity.y += settings.gravity * Time.fixedDeltaTime;
                velocity.y = Mathf.Max(settings.maxFallSpeed, velocity.y);
                pos.y += velocity.y * Time.fixedDeltaTime;
            }

            pos = Collision.ValidatePosition(pos);
            
            character.Rigidbody.MovePosition(pos);
        }

        public void MoveLeft()
        {
            horizontal = HorizontalInput.Left;
        }

        public void MoveRight()
        {
            horizontal = HorizontalInput.Right;
        }

        public void StopMoving()
        {
            horizontal = HorizontalInput.None;
        }

        public void JumpPressed() => jump.JumpRequested();
        public void JumpReleased() => jump.JumpReleased();

        private void OnRecoverStateChanged(bool isRecovering)
        {
            canMove = !isRecovering;
        }
        
        private void OnJumpStateChanged(bool isJumping)
        {
            UpdateFallingState(isJumping, character.State.GetState(CharacterStates.Grounded));
        }
        
        private void OnGroundedStateChanged(bool isGrounded)
        {
            UpdateFallingState(character.State.GetState(CharacterStates.Jumping), isGrounded);

            if (isGrounded)
            {
                var fallHeight = fallStartHeight - character.Transform.position.y;
                if (fallHeight > 6f)
                {
                    character.Effects.FallMajor();
                    character.Afflictions.BeginFallRecovery();
                }
                else if (fallHeight > 3f)
                {
                    character.Effects.FallMinor();
                }
            }
        }

        private void UpdateFallingState(bool isJumping, bool isGrounded)
        {
            if (!isJumping)
            {
                if (!isGrounded)
                {
                    velocity.y = 0f;
                    fallStartHeight = character.Transform.position.y;
                    character.State.SetState(CharacterStates.IsFalling, true);
                }
            }
            else
            {
                character.State.SetState(CharacterStates.IsFalling, false);
            }
        }
    }
}