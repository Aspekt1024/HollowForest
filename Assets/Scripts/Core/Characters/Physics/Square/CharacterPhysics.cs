using System;
using Core.Characters.Physics.Square;
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
            public bool roll;
            
            [Header("Vertical Movement")]
            public float gravity = -60f;
            public float maxFallSpeed = -50f;

            public JumpPhysics.Settings jumpSettings;
            public WallPhysics.Settings wallSettings;
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

        public event Action<Character, Vector3, float> OnGoundHit = delegate { }; 
        public event Action<Character, Vector3> OnAttachedToWall = delegate { }; 

        private readonly Character character;
        private readonly Settings settings;

        private readonly JumpPhysics jump;
        private readonly WallPhysics wall;
        private readonly RollingPhysics rolling;

        private float timeHorizontalVelocityOverrideEnds;
        private float horizontalVelocityOverride;
        
        public CollisionSensor Collision { get; }

        public CharacterPhysics(Character character, Settings settings)
        {
            this.character = character;
            this.settings = settings;

            Collision = new CollisionSensor(character);
            jump = new JumpPhysics(character, settings.jumpSettings, Collision);
            wall = new WallPhysics(character, settings.wallSettings, Collision);
            
            rolling = new RollingPhysics(character);

            character.State.RegisterStateObserver(CharacterStates.IsJumping, OnJumpStateChanged);
            character.State.RegisterStateObserver(CharacterStates.IsGrounded, OnGroundedStateChanged);
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
            var startPos = character.Rigidbody.position;
            var pos = startPos;
            velocity = character.Rigidbody.velocity;

            if (canMove && !wall.IsAttachedToWall)
            {
                if (Time.time >= timeHorizontalVelocityOverrideEnds)
                {
                    CalculateHorizontalMovementVelocity();
                }
                else
                {
                    velocity.x = horizontalVelocityOverride;
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
            else if (!wall.IsAttachedToWall)
            {
                if (velocity.y > 0) velocity.y = 0;
                velocity.y += settings.gravity * Time.fixedDeltaTime;
                velocity.y = Mathf.Max(settings.maxFallSpeed, velocity.y);
                pos.y += velocity.y * Time.fixedDeltaTime;
            }

            pos = Collision.ProcessBounds(pos);
            
            velocity = (pos - startPos) / Time.fixedDeltaTime;

            character.Rigidbody.velocity = velocity;
            if (settings.roll)
            {
                character.Rigidbody.angularVelocity = -Mathf.Sign(velocity.x) * 2 * Mathf.PI * velocity.x * velocity.x;
            }
        }

        public void SetHorizontalVelocity(float xVelocity, float duration)
        {
            timeHorizontalVelocityOverrideEnds = Time.time + duration;
            horizontalVelocityOverride = xVelocity;
        }

        private void CalculateHorizontalMovementVelocity()
        {
            // TODO lerp movement speed
            var newVelocity = velocity;
            switch (horizontal)
            {
                case HorizontalInput.Left:
                    newVelocity.x = -settings.moveSpeed;
                    break;
                case HorizontalInput.Right:
                    newVelocity.x = settings.moveSpeed;
                    break;
                default:
                    newVelocity.x = 0f;
                    break;
            }
            
            if (settings.roll)
            {
                newVelocity = rolling.CalculateVelocity(velocity, newVelocity, Collision.CurrentGradient);
            }

            velocity = newVelocity;
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
            UpdateFallingState(isJumping, character.State.GetState(CharacterStates.IsGrounded));
        }
        
        private void OnGroundedStateChanged(bool isGrounded)
        {
            if (isGrounded)
            {
                var fallHeight = character.State.GetState(CharacterStates.IsFalling) ? fallStartHeight - character.Transform.position.y : 0f;
                OnGoundHit?.Invoke(character, Collision.CurrentGroundPoint, fallHeight);
                if (fallHeight > 6f)
                {
                    character.Afflictions.BeginFallRecovery();
                }
            }
            
            UpdateFallingState(character.State.GetState(CharacterStates.IsJumping), isGrounded);
        }

        private void UpdateFallingState(bool isJumping, bool isGrounded)
        {
            if (!isJumping)
            {
                if (!isGrounded)
                {
                    velocity.y = 0f;
                    fallStartHeight = character.Transform.position.y;
                }
                character.State.SetState(CharacterStates.IsFalling, !isGrounded);
            }
            else
            {
                character.State.SetState(CharacterStates.IsFalling, false);
            }
        }
    }
}