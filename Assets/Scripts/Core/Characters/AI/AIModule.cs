using System.Collections.Generic;
using HollowForest.AI.States;
using UnityEngine;

namespace HollowForest.AI
{
    [CreateAssetMenu(menuName = "Game/AI/AI Module", fileName = "NewAIModule")]
    public class AIModule : ScriptableObject
    {
        public string defaultActionGuid;
        public List<AIAction.Transition> interrupts = new List<AIAction.Transition>();
        public List<AIAction> actions = new List<AIAction>();
    }
}