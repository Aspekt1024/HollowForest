using HollowForest.Combat;

namespace HollowForest.AI
{
    public class HealthSensor : AISensor, Health.IDamageObserver
    {
        protected override void OnInit()
        {
            agent.character.Health.RegisterObserver(this);
        }
        
        public void OnDamageTaken(HitDetails hitDetails)
        {
            agent.character.Director.BlockInputs(0.8f);
            if (hitDetails.source != null)
            {
                agent.memory.SetObject(AIObject.PotentialThreat, hitDetails.source);
            }
        }
    }
}