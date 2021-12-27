using UnityEngine;

namespace HollowForest.AI
{
    public class ThreatSensor : AISensor
    {
        public float threatLostDistance = 10f;
        
        private Character currentThreat;
        private bool isLockedOn;

        protected override void OnInit()
        {
            agent.memory.RegisterObjectObserver(AIObject.PotentialThreat, OnPotentialThreatUpdated);
            agent.memory.RegisterObjectObserver(AIObject.LockedOnThreat, OnLockedOnThreatUpdated);
        }

        private void Update()
        {
            CheckThreatConditions();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var character = other.GetComponent<Character>();
            RegisterThreat(character, false);
        }

        private bool IsValidThreat(Character character)
        {
            if (character == null || !character.IsPlayer()) return false;
            return character.State.GetState(CharacterStates.IsAlive);
        }

        private void RegisterThreat(Character threat, bool lockOn)
        {
            if (currentThreat != null && isLockedOn) return;
            if (!IsValidThreat(threat)) return;
            
            currentThreat = threat;
            isLockedOn = lockOn;
            agent.memory.SetState(AIState.HasThreat, true);
            agent.memory.SetObject(AIObject.Threat, threat);
        }

        private void UnregisterThreat()
        {
            agent.memory.SetState(AIState.HasThreat, false);
            agent.memory.SetObject(AIObject.Threat, null);
            currentThreat = null;
        }

        private void CheckThreatConditions()
        {
            if (currentThreat == null) return;

            if (!IsValidThreat(currentThreat))
            {
                UnregisterThreat();
                return;
            }

            if (!isLockedOn)
            {
                var distVector = currentThreat.transform.position - agent.character.transform.position;
                distVector.z = 0f;
                if (distVector.sqrMagnitude > threatLostDistance * threatLostDistance)
                {
                    UnregisterThreat();
                }
            }
        }

        private void OnPotentialThreatUpdated(object other)
        {
            if (!(other is Character character)) return;
            RegisterThreat(character, false);
        }

        private void OnLockedOnThreatUpdated(object other)
        {
            RegisterThreat(other as Character, true);
        }
    }
}