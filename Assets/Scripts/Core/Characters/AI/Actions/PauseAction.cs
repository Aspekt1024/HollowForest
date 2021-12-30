using UnityEngine;

namespace HollowForest.AI
{
    public class PauseAction : AIAction
    {
        public float pauseDuration = 1f;
        
        public override string DisplayName => "Pause";
        public override string MenuCategory => "Misc";

        private float timeComplete;
        
        protected override void SetupPreconditions()
        {
            
        }

        protected override void OnStart()
        {
            timeComplete = Time.time + pauseDuration;
        }

        protected override void OnStop()
        {
            
        }

        protected override void OnTick()
        {
            if (Time.time >= timeComplete)
            {
                ActionComplete();
            }
        }
    }
}