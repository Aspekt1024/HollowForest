using HollowForest.World;
using UnityEngine;

namespace HollowForest.AI
{
    public class FloatAction : AIAction
    {
        public float horizontalSpeed = 2f;
        public float verticalSpeed = 2f;
        
        public override string DisplayName => "Float";
        public override string MenuCategory => "Movement";

        private bool isMovingRight;
        private bool isMovingUp;
        
        protected override void SetupPreconditions()
        {
            
        }

        protected override void OnStart()
        {
            RegisterObservations();
            BeginMovement();
        }

        protected override void OnStop()
        {
            UnregisterObservations();
        }

        protected override void OnTick()
        {
            
        }

        private void RegisterObservations()
        {
            Agent.character.Physics.Collision.OnCeilingHit += OnCeilingHit;
            Agent.character.Physics.Collision.OnWallHit += OnWallHit;
            Agent.character.State.RegisterStateObserver(CharacterStates.IsGrounded, OnGroundStateChanged);
        }

        private void UnregisterObservations()
        {
            Agent.character.Physics.Collision.OnCeilingHit -= OnCeilingHit;
            Agent.character.Physics.Collision.OnWallHit -= OnWallHit;
            Agent.character.State.UnregisterStateObserver(CharacterStates.IsGrounded, OnGroundStateChanged);
        }

        private void OnCeilingHit()
        {
            ReverseVerticalDirection();
        }

        private void OnWallHit(Vector3 wallPos, Surface surface)
        {
            ReverseHorizontalDirection();
        }

        private void OnGroundStateChanged(bool isGrounded)
        {
            if (!isGrounded) return;
            ReverseVerticalDirection();
        }

        private void ReverseHorizontalDirection()
        {
            isMovingRight = !isMovingRight;
            BeginMovement();
        }

        private void ReverseVerticalDirection()
        {
            isMovingUp = !isMovingUp;
             BeginMovement();
        }

        private void BeginMovement()
        {
            if (isMovingRight)
            {
                Agent.character.Physics.MoveRight(horizontalSpeed);
            }
            else
            {
                Agent.character.Physics.MoveLeft(horizontalSpeed);
            }
            
            if (isMovingUp)
            {
                Agent.character.Physics.SetVerticalMovement(verticalSpeed);
            }
            else
            {
                Agent.character.Physics.SetVerticalMovement(-verticalSpeed);
            }
        }
    }
}