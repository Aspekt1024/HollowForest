using System;
using System.Collections.Generic;

namespace HollowForest
{
    public enum CharacterStates
    {
        IsGrounded,
        IsJumping,
        IsFalling,
        IsRecovering,
        IsAttachedToWall,
    }
    
    public class CharacterState
    {
        private readonly Dictionary<CharacterStates, bool> stateDict = new Dictionary<CharacterStates, bool>();

        private readonly Dictionary<CharacterStates, List<Action<bool>>> observations = new Dictionary<CharacterStates, List<Action<bool>>>();
        private readonly List<Action<CharacterStates, bool>> allStateObservations = new List<Action<CharacterStates, bool>>();

        public void RegisterStateObserver(CharacterStates state, Action<bool> stateChangeCallback)
        {
            if (!observations.ContainsKey(state))
            {
                observations.Add(state, new List<Action<bool>>());
            } 
            observations[state].Add(stateChangeCallback);
        }

        public void UnregisterStateObserver(CharacterStates state, Action<bool> stateChangeCallback)
        {
            if (!observations.ContainsKey(state)) return;
            var index = observations[state].FindIndex(c => c == stateChangeCallback);
            if (index >= 0)
            {
                observations[state].RemoveAt(index);
            }
        }

        public void RegisterAllStateObserver(Action<CharacterStates, bool> stateChangeCallback)
        {
            allStateObservations.Add(stateChangeCallback);
        }

        public void UnregisterAllStateObserver(Action<CharacterStates, bool> stateChangeCallback)
        {
            var index = allStateObservations.FindIndex(o => o == stateChangeCallback);
            if (index >= 0)
            {
                allStateObservations.RemoveAt(index);
            }
        }

        /// <summary>
        /// Returns a copy of the entire character state dictionary.
        /// </summary>
        public Dictionary<CharacterStates, bool> GetStateCopy() => new Dictionary<CharacterStates, bool>(stateDict);
        
        public void SetState(CharacterStates state, bool value)
        {
            if (stateDict.ContainsKey(state))
            {
                if (stateDict[state] != value)
                {
                    stateDict[state] = value;
                    NotifyStateChange(state, value);
                }
            }
            else
            {
                stateDict.Add(state, value);
                NotifyStateChange(state, value);
            }
        }

        /// <summary>
        /// Returns value of the given state. If the state does not exist yet, returns false.
        /// </summary>
        public bool GetState(CharacterStates state)
        {
            if (stateDict.ContainsKey(state))
            {
                return stateDict[state];
            }

            return false;
        }

        private void NotifyStateChange(CharacterStates state, bool value)
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