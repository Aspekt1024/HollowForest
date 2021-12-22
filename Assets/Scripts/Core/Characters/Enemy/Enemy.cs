using System;
using HollowForest.AI;

namespace HollowForest
{
    public class Enemy : Character
    {
        public AIModule module;
        
        private AIAgent ai;

        public event Action<Enemy> Defeated = delegate { };

        private void Start()
        {
            ai = new AIAgent(this);
            var sensors = GetComponentsInChildren<AISensor>();
            foreach (var sensor in sensors)
            {
                ai.RegisterSensor(sensor);
            }
            
            State.RegisterStateObserver(CharacterStates.IsAlive, OnAliveStateChanged);
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

        public void Engage(Character character) => ai.Engage(character);
        
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