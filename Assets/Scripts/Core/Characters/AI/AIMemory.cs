using System;
using System.Collections.Generic;

namespace HollowForest.AI
{
    public enum AIState
    {
        HasThreat,
        IsNearLeftEdge,
        IsNearRightEdge,
    }
    
    public enum AIObject
    {
        Threat,
    }
    
    public class AIMemory
    {
        private readonly Dictionary<AIState, bool> states = new Dictionary<AIState, bool>();
        private readonly Dictionary<AIObject, object> objects = new Dictionary<AIObject, object>();
        
        private readonly Dictionary<AIState, List<Action<bool>>> observations = new Dictionary<AIState, List<Action<bool>>>();
        private readonly List<Action<AIState, bool>> allStateObservations = new List<Action<AIState, bool>>();

        public void RegisterStateObserver(AIState state, Action<bool> stateChangeCallback)
        {
            if (!observations.ContainsKey(state))
            {
                observations.Add(state, new List<Action<bool>>());
            }
            observations[state].Add(stateChangeCallback);
        }

        public void UnregisterStateObserver(AIState state, Action<bool> stateChangeCallback)
        {
            if (!observations.ContainsKey(state)) return;
            var index = observations[state].FindIndex(c => c == stateChangeCallback);
            if (index >= 0)
            {
                observations[state].RemoveAt(index);
            }
        }
        
        public void RegisterAllStateObserver(Action<AIState, bool> stateChangeCallback)
        {
            allStateObservations.Add(stateChangeCallback);
        }

        public void UnregisterAllStateObserver(Action<AIState, bool> stateChangeCallback)
        {
            var index = allStateObservations.FindIndex(o => o == stateChangeCallback);
            if (index >= 0)
            {
                allStateObservations.RemoveAt(index);
            }
        }
        
        public void SetState(AIState state, bool value)
        {
            if (states.ContainsKey(state))
            {
                if (states[state] != value)
                {
                    states[state] = value;
                    NotifyStateChange(state, value);
                }
            }
            else
            {
                states.Add(state, value);
                NotifyStateChange(state, value);
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

        private void NotifyStateChange(AIState state, bool value)
        {
            for (int i = allStateObservations.Count - 1; i >= 0; i--)
            {
                allStateObservations[i].Invoke(state, value);
            }
            
            if (!observations.ContainsKey(state)) return;

            for (int i = observations[state].Count - 1; i >= 0; i--)
            {
                observations[state][i].Invoke(value);
            }
        }
    }
}