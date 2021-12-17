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

        public event Action<Enemy> Defeated = delegate { };

        private void Awake()
        {
            sensor.InitAwake(this);
        }

        private void Start()
        {
            ai = new AIAgent(character);
            character.State.RegisterStateObserver(CharacterStates.IsAlive, OnAliveStateChanged);
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
        
        private void OnAliveStateChanged(bool isAlive)
        {
            if (!isAlive)
            {
                Defeated?.Invoke(this);
            }
        }
    }
}