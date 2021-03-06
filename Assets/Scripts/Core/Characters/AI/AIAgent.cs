using System;
using HollowForest.Combat;

namespace HollowForest.AI
{
    public class AIAgent
    {
        private bool isEnabled;

        public readonly Character character;
        public readonly AIMemory memory;
        
        private readonly AIExecutor executor;

        public AIModule GetRunningModule() => executor.GetRunningModule();
        
        public AIAgent(Character character)
        {
            this.character = character;

            memory = new AIMemory();
            executor = new AIExecutor(this);
        }

        public void Run(AIModule module)
        {
            isEnabled = true;
            executor.Run(module);
        }

        public void Stop()
        {
            isEnabled = false;
            executor.Stop();
        }

        public void Tick()
        {
            if (!isEnabled) return;
            executor.Tick();
        }

        public void RegisterSensor(AISensor sensor)
        {
            sensor.Init(this);
        }

        public void Engage(Character threat, bool lockOn)
        {
            memory.SetObject(lockOn ? AIObject.LockedOnThreat : AIObject.PotentialThreat, threat);
        }

        public void RegisterActionObserver(Action<AIExecutor.TransitionInfo> callback) => executor.OnTransition += callback;
        public void UnregisterActionObserver(Action<AIExecutor.TransitionInfo> callback) => executor.OnTransition -= callback;

    }
}