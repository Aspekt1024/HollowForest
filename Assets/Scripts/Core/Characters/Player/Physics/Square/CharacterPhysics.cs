using System;
using System.Collections.Generic;
using System.Linq;
using Core.Characters.Physics.Square;
using HollowForest.Physics;
using HollowForest.World;
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
            public DashPhysics.Settings dashSettings;
            public WallPhysics.Settings wallSettings;
        }

        private enum HorizontalInput
        {
            None,
            Left,
            Right,
        }

        private HorizontalInput horizontal;

        private enum MovementBlocks
        {
            Recovering, Attacking, Master,
        }
        private readonly HashSet<MovementBlocks> movementBlockList = new HashSet<MovementBlocks>();

        private Vector2 velocity;
        private float fallStartHeight;

        public event Action<Vector3, float> OnGoundHit = delegate { }; 

        private readonly Character character;
        private readonly Settings settings;

        private readonly JumpPhysics jump;
        private readonly DashPhysics dash;
        private readonly WallPhysics wall;
        private readonly RollingPhysics rolling;

        private float timeVelocityOverrideEnds;
        private Vector3 velocityOverride;
        
        private float timeHorizontalVelocityOverrideEnds;
        private float horizontalVelocityOverride;

        private float moveSpeed;
        private float verticalSpeed;
        private bool isVerticalSpeedOverridden;
        
        public CollisionSensor Collision { get; }
        private Rigidbody2D body;

        public CharacterPhysics(Character character, Settings settings)
        {
            this.character = character;
            this.settings = settings;

            Collision = new CollisionSensor(character);
            body = character.GetComponent<Rigidbody2D>();
            jump = new JumpPhysics(character, settings.jumpSettings, Collision);
            dash = new DashPhysics(character, settings.dashSettings, Collision);
            wall = new WallPhysics(character, settings.wallSettings, Collision);
            
            rolling = new RollingPhysics(character);

            character.State.RegisterStateObserver(CharacterStates.IsJumping, OnJumpStateChanged);
            character.State.RegisterStateObserver(CharacterStates.IsDashing, OnDashStateChanged);
            character.State.RegisterStateObserver(CharacterStates.IsGrounded, OnGroundedStateChanged);
            character.State.RegisterStateObserver(CharacterStates.IsRecovering, isRecovering => OnMovementLockStateChanged(MovementBlocks.Recovering, isRecovering));
            character.State.RegisterStateObserver(CharacterStates.IsLockedForAttack, isLocked => OnMovementLockStateChanged(MovementBlocks.Attacking, isLocked));

            Collision.OnCeilingHit += CancelMovementOverride;
            Collision.OnWallHit += CancelMovementOverride;

            // Set initial conditions
            SetOnGround();
        }

        private void CancelMovementOverride() => timeHorizontalVelocityOverrideEnds = Time.time - 0.1f;
        private void CancelMovementOverride(Vector3 wallPos, Surface surface) => CancelMovementOverride();

        public void SetOnGround(Vector3 position)
        {
            character.transform.position = position;
            body.MovePosition(position);
            SetOnGround();
        }
        
        private void SetOnGround()
        {
            var extent = character.Collider.bounds.extents.y;
            var pos = character.Transform.position;
            var hit = Physics2D.Raycast(pos, Vector2.down, 10f, 1 << LayerMask.NameToLayer("World"));
            if (hit.collider != null)
            {
                pos.y = hit.point.y + extent - character.Collider.offset.y;
                character.Transform.position = pos;
            }
            velocity = Vector2.zero;
        }

        public void Tick_Fixed()
        {   
            character.Rigidbody.velocity = CalculateVelocity();
            if (settings.roll)
            {
                character.Rigidbody.angularVelocity = -Mathf.Sign(velocity.x) * 2 * Mathf.PI * velocity.x * velocity.x;
            }
        }

        public void OverrideVelocity(Vector3 velocityOverride, float duration)
        {
            this.velocityOverride = velocityOverride;
            timeVelocityOverrideEnds = Time.time + duration;
        }

        public void SetHorizontalVelocity(float xVelocity, float duration)
        {
            timeHorizontalVelocityOverrideEnds = Time.time + duration;
            horizontalVelocityOverride = xVelocity;
        }

        public void SetVerticalMovement(float speed)
        {
            isVerticalSpeedOverridden = true;
            verticalSpeed = speed;
        }

        private Vector3 CalculateVelocity()
        {
            if (Time.time < timeVelocityOverrideEnds)
            {
                velocity = velocityOverride;
            }
            else
            {
                velocity = CalculateHorizontalInfluencedVelocity(character.Rigidbody.velocity);
            
                if (dash.IsDashing)
                {
                    velocity = dash.CalculateVelocity(velocity);
                }
            }
            
            var startPos = character.Rigidbody.position;
            var pos = startPos;
            pos.x += velocity.x * Time.fixedDeltaTime;
            pos.y = CalculateHeight(velocity, pos);

            pos = Collision.ProcessBounds(pos);
            velocity = (pos - startPos) / Time.fixedDeltaTime;
            
            return velocity;
        }

        private float CalculateHeight(Vector3 velocity, Vector3 pos)
        {
            if (dash.IsDashing)
            {
                pos.y = dash.CalculateHeight(velocity, pos);
            }
            else if (jump.IsJumping)
            {
                pos.y = jump.CalculateHeight();
            }
            else if (!wall.IsAttachedToWall)
            {
                if (isVerticalSpeedOverridden)
                {
                    velocity.y = verticalSpeed;
                }
                else
                {
                    if (velocity.y > 0) velocity.y = 0;
                    velocity.y += settings.gravity * Time.fixedDeltaTime;
                    velocity.y = Mathf.Max(settings.maxFallSpeed, velocity.y);
                }
                pos.y += velocity.y * Time.fixedDeltaTime;
            }

            return pos.y;
        }

        private Vector3 CalculateHorizontalInfluencedVelocity(Vector3 velocity)
        {
            if (Time.time < timeHorizontalVelocityOverrideEnds)
            {
                velocity.x = horizontalVelocityOverride;
                return velocity;
            }
            
            if (movementBlockList.Any() || wall.IsAttachedToWall)
            {
                velocity.x = 0;
                return velocity;
            }
            
            // TODO lerp movement speed
            var newVelocity = velocity;
            switch (horizontal)
            {
                case HorizontalInput.Left:
                    newVelocity.x = -moveSpeed;
                    break;
                case HorizontalInput.Right:
                    newVelocity.x = moveSpeed;
                    break;
                default:
                    newVelocity.x = 0f;
                    break;
            }
            
            if (settings.roll)
            {
                newVelocity = rolling.CalculateVelocity(velocity, newVelocity, Collision.CurrentGradient);
            }

            return newVelocity;
        }

        public void BlockInput()
        {
            movementBlockList.Add(MovementBlocks.Master);
            dash.CancelDash();
            jump.JumpReleased();
        }

        public void ResumeInput()
        {
            movementBlockList.Remove(MovementBlocks.Master);
        }

        public void MoveLeft(float speed = -1f)
        {
            moveSpeed = speed > 0 ? speed : settings.moveSpeed;
            horizontal = HorizontalInput.Left;
            character.State.SetState(CharacterStates.IsFacingRight, false);
            character.Animator.MoveLeft();
        }

        public void MoveRight(float speed = -1f)
        {
            moveSpeed = speed > 0 ? speed : settings.moveSpeed;
            horizontal = HorizontalInput.Right;
            character.State.SetState(CharacterStates.IsFacingRight, true);
            character.Animator.MoveRight();
        }

        public void StopMoving()
        {
            horizontal = HorizontalInput.None;
            character.Animator.StopMoving();
            isVerticalSpeedOverridden = false;
        }

        public void JumpPressed() => jump.JumpRequested();
        public void JumpReleased() => jump.JumpReleased();

        public void DashPressed() => dash.DashRequested();
        
        private void OnJumpStateChanged(bool isJumping)
        {
            UpdateFallingState(isJumping, character.State.GetState(CharacterStates.IsGrounded));
        }

        private void OnDashStateChanged(bool isDashing)
        {
            var isJumping = !isDashing && character.State.GetState(CharacterStates.IsJumping);
            UpdateFallingState(isJumping, character.State.GetState(CharacterStates.IsGrounded));
        }
        
        private void OnGroundedStateChanged(bool isGrounded)
        {
            if (isGrounded)
            {
                var fallHeight = character.State.GetState(CharacterStates.IsFalling) ? fallStartHeight - character.Transform.position.y : 0f;
                OnGoundHit?.Invoke(Collision.CurrentGroundPoint, fallHeight);
            }
            
            UpdateFallingState(character.State.GetState(CharacterStates.IsJumping), isGrounded);
        }

        private void OnMovementLockStateChanged(MovementBlocks block, bool isBlocked)
        {
            if (isBlocked)
            {
                movementBlockList.Add(block);
            }
            else
            {
                movementBlockList.Remove(block);
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