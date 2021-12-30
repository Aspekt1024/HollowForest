using UnityEngine;

namespace HollowForest.AI
{
    public class ThreatDistanceSensor : AISensor
    {   
        private bool hasThreat;
        private Transform threatTf;

        private float threatDistanceThreshold = 1f;
        private bool isLineOfSightRequired;
        private bool isHorizontalPlaneOnly;
        
        protected override void OnInit()
        {
            agent.memory.RegisterObjectObserver(AIObject.Threat, OnThreatChanged);
        }

        public void SetDistanceThreshold(float distance, bool isLineOfSightRequired, bool isHorizontalPlaneOnly)
        {
            threatDistanceThreshold = distance;
            this.isLineOfSightRequired = isLineOfSightRequired;
            this.isHorizontalPlaneOnly = isHorizontalPlaneOnly;
        }

        private void OnThreatChanged(object threat)
        {
            if (threat is Character c)
            {
                hasThreat = true;
                threatTf = c.Transform;
            }
            else
            {
                hasThreat = false;
                threatTf = null;
            }
        }

        private void Update()
        {
            if (hasThreat)
            {
                CheckThreatDistance();
            }
        }

        private void CheckThreatDistance()
        {
            var threatPos = threatTf.position;
            var pos = transform.position;

            var distVector = threatPos - pos;
            var distance = isHorizontalPlaneOnly ? Mathf.Abs(distVector.x) : distVector.magnitude;
            
            var isWithinDistance = isHorizontalPlaneOnly
                ? Mathf.Abs(distVector.x) > Mathf.Abs(distVector.y) && distance < threatDistanceThreshold
                : distance < threatDistanceThreshold;
            
            if (isWithinDistance)
            {
                isWithinDistance = !isLineOfSightRequired || AIUtil.IsInLineOfSight(pos, threatPos);
                agent.memory.SetState(AIState.IsThreatWithinDistance, isWithinDistance);
            }
            else
            {
                agent.memory.SetState(AIState.IsThreatWithinDistance, false);
            }
            
            agent.memory.SetObject(AIObject.ThreatDistance, Mathf.Abs(distance));
        }
    }
}