using HollowForest.Combat;

namespace HollowForest.AI
{
    public class AIAgent
    {
        private bool isEnabled;

        public readonly Character character;
        public readonly AIMemory memory;
        
        private readonly AIExecutor executor;
        
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

        public void Engage(Character threat)
        {
            memory.SetState(AIState.HasThreat, true);
            memory.SetObject(AIObject.Threat, threat);
        }

    }
}