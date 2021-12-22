using System.Collections.Generic;
using HollowForest.AI.States;
using UnityEngine;

namespace HollowForest.AI
{
    [CreateAssetMenu(menuName = "Game/AI/AI Module", fileName = "NewAIModule")]
    public class AIModule : ScriptableObject
    {
        public int defaultActionID;
        public List<AIAction.Transition> interrupts;
        public List<AIAction> actions;
    }
}