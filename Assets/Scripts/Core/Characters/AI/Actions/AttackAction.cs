using UnityEngine;

namespace HollowForest.AI.States
{
    [CreateAssetMenu(menuName = "Game/AI/Actions/Attack Action", fileName = "AttackAction")]
    public class AttackAction : AIAction
    {
        private Character threat;
        
        protected override void SetupPreconditions()
        {
            AddPrecondition(AIState.HasThreat, true);
        }

        protected override void OnStart()
        {
            threat = Agent.memory.GetObject<Character>(AIObject.Threat);
            if (threat == null)
            {
                ActionFailure();
            }
        }

        protected override void OnStop()
        {
            
        }
        
        protected override void OnTick()
        {
            if (!threat.State.GetState(CharacterStates.IsAlive))
            {
                threat = null;
                return;
            }
            
            var dist = threat.transform.position - Character.transform.position;
            
            if (Mathf.Abs(dist.x) < 2f)
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
                Character.Combat.AttackLightRequested();
            }
            else
            {
                if (dist.x > 0)
                {
                    Character.Physics.MoveRight();
                }
                else
                {
                    Character.Physics.MoveLeft();
                }

                if (Mathf.Abs(dist.x) > 5f)
                {
                    Character.Physics.DashPressed();
                }
            }
        }
    }
}