using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HollowForest.AI
{
    public abstract class AIAction : ScriptableObject
    {
        [Serializable]
        public class Transition
        {
            public List<AIState> pConditions;
            public List<AIState> nConditions;
            public string actionGuid;
            public int priority;

            public Transition(string actionGuid)
            {
                this.actionGuid = actionGuid;
                pConditions = new List<AIState>();
                nConditions = new List<AIState>();
                priority = 1;
            }
        }

        public string guid;
        public List<Transition> transitions = new List<Transition>();

        protected AIAgent Agent { get; private set; }
        protected Character Character => Agent.character;

        private readonly Dictionary<AIState, bool> preconditions = new Dictionary<AIState, bool>();

        public abstract string DisplayName { get; }
        public abstract string MenuCategory { get; }

        public void Init(AIAgent agent)
        {
            Agent = agent;
        }

        public virtual bool CanRun()
        {
            return preconditions.All(p => Agent.memory.IsMatch(p.Key, p.Value));
        }

        public void Start()
        {
            OnStart();
        }

        public void Stop()
        {
            OnStop();
        }

        public void Tick()
        {
            OnTick();
        }

        protected abstract void SetupPreconditions();
        
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract void OnTick();

        protected void AddPrecondition(AIState precondition, bool value)
        {
            preconditions.Add(precondition, value);
        }

        protected void ActionFailure()
        {
            Debug.LogError("Action failed");
        }

    }
}