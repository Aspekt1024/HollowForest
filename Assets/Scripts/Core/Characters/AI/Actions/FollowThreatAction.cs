
using UnityEngine;

namespace HollowForest.AI
{
    public class FollowThreatAction : AIAction
    {
        public float followSpeed = 5f;
        public float followDistance = 2f;
        public bool canDash = true;
        public bool canJump = true;
        
        public override string DisplayName => "Follow Threat";
        public override string MenuCategory => "Movement";

        private Character threat;
        private bool canMove;

        protected override void SetupPreconditions()
        {
            AddPrecondition(AIState.HasThreat, true);
        }
        
        protected override void OnStart()
        {
            canMove = true;
            threat = Agent.memory.GetObject<Character>(AIObject.Threat);
            
            Agent.memory.RegisterStateObserver(AIState.IsNearLeftWall, OnNearWallStateChanged);
            Agent.memory.RegisterStateObserver(AIState.IsNearRightWall, OnNearWallStateChanged);
            Agent.character.State.RegisterStateObserver(CharacterStates.IsJumping, OnJumpStateChanged);
        }

        protected override void OnStop()
        {
            Agent.memory.UnregisterStateObserver(AIState.IsNearLeftWall, OnNearWallStateChanged);
            Agent.memory.UnregisterStateObserver(AIState.IsNearRightWall, OnNearWallStateChanged);
            Agent.character.State.UnregisterStateObserver(CharacterStates.IsJumping, OnJumpStateChanged);
        }

        protected override void OnTick()
        {
            if (threat == null)
            {
                ActionFailure();
                return;
            }
            
            if (!threat.State.GetState(CharacterStates.IsAlive))
            {
                threat = null;
                return;
            }

            if (!canMove) return;
            
            var dist = threat.transform.position - Character.transform.position;
            
            if (Mathf.Abs(dist.x) < followDistance && dist.y > 0.5f)
            {
                Character.Physics.StopMoving();
                if (dist.x > 0)
                {
                    Character.Animator.MoveRight();
                }
                else
                {
                    Character.Animator.MoveLeft();
                }
            }
            else
            {
                if (dist.x > 0)
                {
                    Character.Physics.MoveRight(followSpeed);
                }
                else
                {
                    Character.Physics.MoveLeft(followSpeed);
                }

                if (canDash && Mathf.Abs(dist.x) > 5f && HasLineOfSight()) // TODO check if dashing will get us across gaps
                {
                    Character.Physics.DashPressed();
                }
            }
        }
        
        private void OnNearWallStateChanged(bool isNearWall)
        {
            canMove = !isNearWall;
            if (!canMove)
            {
                Character.Physics.StopMoving();
            }
            
            if (isNearWall && canJump && !HasLineOfSight())
            {
                Agent.character.Director.JumpPressed();
            }
        }

        private void OnJumpStateChanged(bool isJumping)
        {
            if (!isJumping)
            {
                Agent.character.Director.JumpReleased();
            }
        }

        private bool HasLineOfSight()
        {
            return AIUtil.IsInLineOfSight(Agent.character.transform.position, threat.Transform.position);
        }
    }
}