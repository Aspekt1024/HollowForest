using System;
using System.Collections.Generic;
using UnityEngine;

namespace HollowForest.AI
{
    [CreateAssetMenu(menuName = "Game/AI Module", fileName = "NewAIModule")]
    public class AIModule : ScriptableObject
    {
        public string moduleGuid = Guid.NewGuid().ToString();
        public string defaultActionGuid;
        public List<AIAction.Transition> interrupts = new List<AIAction.Transition>();
        public List<AIAction> actions = new List<AIAction>();

        public bool IsRuntimeInstance { get; private set; }
        [NonSerialized] public readonly List<AIAction> runningActions = new List<AIAction>();
        
        public void Init()
        {
            IsRuntimeInstance = true;
            foreach (var action in actions)
            {
                runningActions.Add(Instantiate(action));
            }
        }
    }
}