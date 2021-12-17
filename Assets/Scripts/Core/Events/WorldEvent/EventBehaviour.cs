using UnityEngine;

namespace HollowForest.Events
{
    public abstract class EventBehaviour : MonoBehaviour
    {
        public abstract void OnWorldEventTriggered(WorldEvent worldEvent, Character character);
        public abstract void OnWorldEventBegin();
        public abstract void OnWorldEventComplete();
    }
}