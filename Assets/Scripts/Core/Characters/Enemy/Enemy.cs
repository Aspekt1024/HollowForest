using System;
using HollowForest.AI;
using UnityEngine;

namespace HollowForest
{
    public class Enemy : MonoBehaviour
    {
        public Character character;
        public ProximitySensor sensor;

        private AIAgent ai;

        private void Awake()
        {
            sensor.InitAwake(this);
            ai = new AIAgent(character);
        }

        private void FixedUpdate()
        {
            character.Physics.Tick_Fixed();
        }

        private void Update()
        {
            ai.Tick();
        }

        public void ThreatDetected(Character other)
        {
            ai.ThreatDetected(other);
        }
    }
}