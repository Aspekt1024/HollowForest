using UnityEngine;

namespace HollowForest.AI
{
    public static class AIUtil
    {
        public static bool IsInLineOfSight(Vector3 origin, Vector3 target)
        {
            var origin2D = new Vector2(origin.x, origin.y);
            
            var distVector2D = new Vector2(
                target.x - origin.x,
                target.y - origin.y
            );
            
            var mask = Layers.GetLayerMask(Layers.World);
            var hit = Physics2D.Raycast(origin2D, distVector2D, distVector2D.magnitude, mask);
            return hit.collider == null;
        }

        public static bool IsThreatToTheRight(AIAgent agent)
        {
            var target = agent.memory.GetObject<Character>(AIObject.Threat);
            if (target == null)
            {
                Debug.LogError("Null threat on direction check.");
                return true;
            }
            return agent.character.transform.position.x < target.transform.position.x;
        }
    }
}