using UnityEngine;

namespace HollowForest.AI
{
    public class AttackSensor : AISensor
    {
        public float attackDistance = 2f;

        private bool hasThreat;
        private Transform threatTf;

        protected override void OnInit()
        {
            agent.memory.RegisterObjectObserver(AIObject.Threat, OnThreatChanged);
        }

        private void Update()
        {
            if (!hasThreat) return;

            var distVector = threatTf.position - transform.position;

            var isFacingCorrectDirection = agent.character.State.GetState(CharacterStates.IsFacingRight)
                ? distVector.x > 0
                : distVector.x < 0;

            var isInRange = isFacingCorrectDirection && distVector.sqrMagnitude <= attackDistance * attackDistance && Mathf.Abs(distVector.x) > Mathf.Abs(distVector.y);
            agent.memory.SetState(AIState.IsInAttackRange, isInRange);
        }

        private void OnThreatChanged(object newThreat)
        {
            var character = newThreat as Character;
            
            hasThreat = character != null && character.IsPlayer();
            if (hasThreat)
            {
                threatTf = character.Transform;
            }
            else
            {
                threatTf = null;
                agent.memory.SetState(AIState.IsInAttackRange, false);
            }
        }
    }
}