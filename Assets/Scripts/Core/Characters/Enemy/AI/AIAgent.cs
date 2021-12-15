using HollowForest.Combat;
using UnityEngine;

namespace HollowForest.AI
{
    public class AIAgent : Health.IDamageObserver
    {
        private readonly Character character;

        private Character threat;
        private bool isEnabled;
        
        public AIAgent(Character character)
        {
            this.character = character;
            
            character.State.RegisterStateObserver(CharacterStates.IsAlive, OnAliveStateChanged);
            character.Health.RegisterObserver(this);
            isEnabled = true;
        }

        public void ThreatDetected(Character other)
        {
            if (character == other) return;
            threat = other;
        }

        public void Tick()
        {
            if (!isEnabled) return;
            
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
            if (!threat.State.GetState(CharacterStates.IsAlive))
            {
                threat = null;
                return;
            }
            
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

        private void OnAliveStateChanged(bool isAlive)
        {
            if (!isAlive)
            {
                isEnabled = false;
            }
        }

        public void OnDamageTaken(HitDetails hitDetails)
        {
            character.Director.StopMoving();
            character.Director.BlockInputs(0.8f);
            if (hitDetails.source != null)
            {
                ThreatDetected(hitDetails.source);
            }
        }
    }
}