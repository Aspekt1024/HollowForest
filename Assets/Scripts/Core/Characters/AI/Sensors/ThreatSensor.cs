using System.Collections.Generic;
using HollowForest.AI;
using UnityEngine;

namespace HollowForest
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
            if (character != null)
            {
                RegisterThreat(character);
            }
        }

        private void RegisterThreat(Character threat)
        {
            currentThreat = threat;
            observers.ForEach(o => o.OnThreatDetected(threat));
        }

        private void CheckThreatConditions()
        {
            if (currentThreat == null) return;
            
            // TODO check distance
            
        }
    }
}