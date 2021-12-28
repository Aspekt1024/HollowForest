using System;
using System.Collections.Generic;
using UnityEngine;

namespace HollowForest.AI
{
    [CreateAssetMenu(menuName = "Game/AI/AI Module", fileName = "NewAIModule")]
    public class AIModule : ScriptableObject
    {
        public string defaultActionGuid;
        public List<AIAction.Transition> interrupts = new List<AIAction.Transition>();
        public List<AIAction> actions = new List<AIAction>();

        [NonSerialized] public readonly List<AIAction> runningActions = new List<AIAction>();
        
        public void Init()
        {
            foreach (var action in actions)
            {
                runningActions.Add(Instantiate(action));
            }
        }
    }
}