using System;
using System.Collections.Generic;

namespace HollowForest.AI
{
    public enum AIState
    {
        HasThreat = 1000,
        IsInAttackRange = 1100,
        IsThreatWithinDistance = 1200,
        
        IsNearLeftEdge = 9000,
        IsNearRightEdge = 9010,
        IsNearLeftWall = 9020,
        IsNearRightWall = 9030,
    }
    
    public enum AIObject
    {
        Threat = 1000,
        PotentialThreat = 1100,
        LockedOnThreat = 1200,
        ThreatDistance = 1300,
    }
    
    public class AIMemory
    {
        private readonly Dictionary<AIState, bool> states = new Dictionary<AIState, bool>();
        private readonly Dictionary<AIObject, object> objects = new Dictionary<AIObject, object>();
        
        private readonly Dictionary<AIState, List<Action<bool>>> stateObservations = new Dictionary<AIState, List<Action<bool>>>();
        private readonly List<Action<AIState, bool>> allStateObservations = new List<Action<AIState, bool>>();
        
        private readonly Dictionary<AIObject, List<Action<object>>> objectObservations = new Dictionary<AIObject, List<Action<object>>>();
        private readonly List<Action<AIObject, object>> allObjectObservations = new List<Action<AIObject, object>>();

        public Dictionary<AIState, bool> GetStateCopy() => new Dictionary<AIState, bool>(states);
        public Dictionary<AIObject, object> GetObjectsCopy() => new Dictionary<AIObject, object>(objects);

        public void RegisterStateObserver(AIState state, Action<bool> stateChangeCallback)
        {
            if (!stateObservations.ContainsKey(state))
            {
                stateObservations.Add(state, new List<Action<bool>>());
            }
            stateObservations[state].Add(stateChangeCallback);
        }

        public void UnregisterStateObserver(AIState state, Action<bool> stateChangeCallback)
        {
            if (!stateObservations.ContainsKey(state)) return;
            var index = stateObservations[state].FindIndex(c => c == stateChangeCallback);
            if (index >= 0)
            {
                stateObservations[state].RemoveAt(index);
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

        public void RegisterObjectObserver(AIObject aiObject, Action<object> objectChangeCallback)
        {
            if (!objectObservations.ContainsKey(aiObject))
            {
                objectObservations.Add(aiObject, new List<Action<object>>());
            }
            objectObservations[aiObject].Add(objectChangeCallback);
        }

        public void UnregisterObjectObserver(AIObject aiObject, Action<object> objectChangeCallback)
        {
            if (!objectObservations.ContainsKey(aiObject)) return;
            var index = objectObservations[aiObject].FindIndex(c => c == objectChangeCallback);
            if (index >= 0)
            {
                objectObservations[aiObject].RemoveAt(index);
            }
        }

        public void RegisterAllObjectObserver(Action<AIObject, object> objectChangeCallback)
        {
            allObjectObservations.Add(objectChangeCallback);
        }
        
        public void UnregisterAllObjectObserver(Action<AIObject, object> objectChangeCallback)
        {
            var index = allObjectObservations.FindIndex(o => o == objectChangeCallback);
            if (index >= 0)
            {
                allObjectObservations.RemoveAt(index);
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
            if (objects.ContainsKey(aiObject))
            {
                if (objects[aiObject] != obj)
                {
                    objects[aiObject] = obj;
                    NotifyObjectChange(aiObject, obj);
                }
            }
            else
            {
                objects.Add(aiObject, obj);
                NotifyObjectChange(aiObject, obj);
            }
        }

        public bool IsTrue(AIState state) => states.ContainsKey(state) && states[state];
        public bool GetState(AIState state) => IsTrue(state);

        public bool IsMatch(AIState state, bool value)
        {
            if (!states.ContainsKey(state)) return value == false;
            return states[state] == value;
        }

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
            
            if (!stateObservations.ContainsKey(state)) return;

            for (int i = stateObservations[state].Count - 1; i >= 0; i--)
            {
                stateObservations[state][i].Invoke(value);
            }
        }

        private void NotifyObjectChange(AIObject aiObject, object value)
        {
            if (!objectObservations.ContainsKey(aiObject)) return;

            for (int i = objectObservations[aiObject].Count - 1; i >= 0; i--)
            {
                objectObservations[aiObject][i].Invoke(value);
            }
        }
    }
}