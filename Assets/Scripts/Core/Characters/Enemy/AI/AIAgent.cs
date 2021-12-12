using UnityEngine;

namespace HollowForest.AI
{
    public class AIAgent
    {
        private readonly Character character;

        private Character threat;
        
        public AIAgent(Character character)
        {
            this.character = character;
            
        }

        public void ThreatDetected(Character other)
        {
            if (character == other) return;
            threat = other;
        }

        public void Tick()
        {
            if (threat != null)
            {
                AttackThreat();
            }
            else
            {
                IdleBehaviour();
            }
        }

        private void IdleBehaviour()
        {
            
        }

        private void AttackThreat()
        {
            var dist = threat.transform.position - character.transform.position;
            
            if (Mathf.Abs(dist.x) < 2f)
            {
                character.Physics.StopMoving();
                if (dist.x > 0)
                {
                    character.Animator.MoveRight();
                }
                else
                {
                    character.Animator.MoveLeft();
                }
                character.Combat.AttackLightRequested();
            }
            else
            {
                if (dist.x > 0)
                {
                    character.Physics.MoveRight();
                }
                else
                {
                    character.Physics.MoveLeft();
                }

                if (Mathf.Abs(dist.x) > 5f)
                {
                    character.Physics.DashPressed();
                }
            }
        }
    }
}