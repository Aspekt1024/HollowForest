using System;
using HollowForest.AI;

namespace HollowForest
{
    public class Enemy : Character
    {
        public AIModule module;
        
        private AIAgent ai;

        public event Action<Enemy> Defeated = delegate { };

        public AIAgent GetAI() => ai;

        protected override void PostAwake()
        {
            ai = new AIAgent(this);
            var sensors = GetComponentsInChildren<AISensor>();
            foreach (var sensor in sensors)
            {
                ai.RegisterSensor(sensor);
            }
            State.RegisterStateObserver(CharacterStates.IsAlive, OnAliveStateChanged);
        }

        private void Start()
        {
            ai.Run(module);
        }

        private void FixedUpdate()
        {
            Physics.Tick_Fixed();
        }

        protected override void Update()
        {
            base.Update();
            ai.Tick();
        }

        public void Engage(Character character, bool lockOn) => ai.Engage(character, lockOn);
        
        private void OnAliveStateChanged(bool isAlive)
        {
            if (!isAlive)
            {
                ai.Stop();
                Defeated?.Invoke(this);
            }
        }
    }
}