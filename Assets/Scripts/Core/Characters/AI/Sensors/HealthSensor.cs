using HollowForest.AI;
using HollowForest.Combat;

namespace HollowForest
{
    public class HealthSensor : AISensor, Health.IDamageObserver
    {
        protected override void OnInit()
        {
            agent.character.Health.RegisterObserver(this);
        }
        
        public void OnDamageTaken(HitDetails hitDetails)
        {
            agent.character.Director.StopMoving();
            agent.character.Director.BlockInputs(0.8f);
            if (hitDetails.source != null)
            {
                agent.Engage(hitDetails.source);
            }
        }
    }
}