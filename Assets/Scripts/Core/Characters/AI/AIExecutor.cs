using System.Linq;
using HollowForest.AI.States;
using UnityEngine;

namespace HollowForest.AI
{
    public class AIExecutor
    {
        private AIModule module;
        private AIAction currentAction;

        private readonly AIAgent agent;

        private bool isRunning;

        public AIExecutor(AIAgent agent)
        {
            this.agent = agent;
        }
        
        public void Run(AIModule module)
        {
            this.module = module;

            foreach (var action in module.actions)
            {
                action.Init(agent);
                if (action.guid == module.defaultActionGuid)
                {
                    currentAction = action;
                }
            }

            isRunning = true;
            currentAction.Start();
        }

        public void Stop()
        {
            isRunning = false;

            if (currentAction != null)
            {
                currentAction.Stop();
                currentAction = null;
            }
        }

        public void Tick()
        {
            if (!isRunning) return;
            
            AIAction.Transition validTransition = null;
            var priority = -1;
            foreach (var transition in currentAction.transitions)
            {
                if (transition.priority > priority && IsValidTransition(transition))
                {
                    validTransition = transition;
                    priority = transition.priority;
                }
            }

            if (validTransition != null)
            {
                TransitionToAction(validTransition.actionGuid);
                return;
            }
            
            currentAction?.Tick();
            
        }

        private bool IsValidTransition(AIAction.Transition transition)
        {
            var posConditionsMet = transition.pConditions.All(c => agent.memory.IsMatch(c, true));
            if (!posConditionsMet) return false;

            var negConditionsMet = transition.nConditions.All(c => agent.memory.IsMatch(c, false));
            if (!negConditionsMet) return false;
            
            var action = module.actions.FirstOrDefault(a => a.guid == transition.actionGuid);
            return action != null && action.CanRun();
        }

        private void TransitionToAction(string actionGuid)
        {
            var action = module.actions.FirstOrDefault(a => a.guid == actionGuid);
            if (action == null)
            {
                Debug.LogError($"Failed to find action with ID {actionGuid} in module {module.name}");
                return;
            }

            currentAction = action;
            currentAction.Start();
        }
    }
}