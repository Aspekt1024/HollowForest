using System.Collections.Generic;
using UnityEngine;

namespace HollowForest.AI
{
    public class ThreatSensor : AISensor
    {
        public interface IObserver
        {
            void OnThreatDetected(Character threat);
            void OnThreatLost(Character threat);
        }

        private readonly List<IObserver> observers = new List<IObserver>();

        public void RegisterObserver(IObserver observer) => observers.Add(observer);
        public void UnregisterObserver(IObserver observer) => observers.Remove(observer);

        private Character currentThreat;

        protected override void OnInit()
        {
            
        }

        private void Update()
        {
            CheckThreatConditions();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var character = other.GetComponent<Character>();
            if (IsValidThreat(character))
            {
                RegisterThreat(character);
            }
        }

        private bool IsValidThreat(Character character)
        {
            if (character == null || !character.IsPlayer()) return false;
            return character.State.GetState(CharacterStates.IsAlive);
        }

        private void RegisterThreat(Character threat)
        {
            currentThreat = threat;
            agent.memory.SetState(AIState.HasThreat, true);
            agent.memory.SetObject(AIObject.Threat, threat);
            observers.ForEach(o => o.OnThreatDetected(threat));
        }

        private void UnregisterThreat()
        {
            agent.memory.SetState(AIState.HasThreat, false);
            agent.memory.SetObject(AIObject.Threat, null);
            observers.ForEach(o => o.OnThreatLost(currentThreat));
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
            
            // TODO check distance
            
        }
    }
}