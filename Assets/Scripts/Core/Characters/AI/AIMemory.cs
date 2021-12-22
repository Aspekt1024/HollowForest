using System.Collections.Generic;

namespace HollowForest.AI
{
    public enum AIState
    {
        HasThreat,
    }
    
    public enum AIObject
    {
        Threat,
    }
    
    public class AIMemory
    {
        private readonly Dictionary<AIState, bool> states = new Dictionary<AIState, bool>();
        private readonly Dictionary<AIObject, object> objects = new Dictionary<AIObject, object>();

        public void SetState(AIState state, bool value)
        {
            if (!states.ContainsKey(state))
            {
                states.Add(state, value);
            }
            else
            {
                states[state] = value;
            }
        }

        public void SetObject(AIObject aiObject, object obj)
        {
            if (!objects.ContainsKey(aiObject))
            {
                objects.Add(aiObject, obj);
            }
            else
            {
                objects[aiObject] = obj;
            }
        }

        public bool IsTrue(AIState state) => states.ContainsKey(state) && states[state];
        public bool GetState(AIState state) => IsTrue(state);

        public bool IsMatch(AIState state, bool value) => states.ContainsKey(state) && states[state] == value || value == false;

        public bool IsMatch(AIObject aiObject, object obj)
        {
            if (!objects.ContainsKey(aiObject)) return false;
            return objects[aiObject] == obj;
        }

        public T GetObject<T>(AIObject aiObject)
        {
            if (!objects.ContainsKey(aiObject)) return default;
            return (T) objects[aiObject];
        }
    }
}