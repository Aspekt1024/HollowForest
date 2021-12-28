using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HollowForest.AI
{
    public class AIExecutor
    {
        private AIModule module;
        private AIAction currentAction;

        private readonly AIAgent agent;

        private bool isRunning;
        private bool isInterruptCheckRequired;

        public AIExecutor(AIAgent agent)
        {
            this.agent = agent;
            agent.memory.RegisterAllStateObserver(OnAIMemoryChanged);
        }
        
        public void Run(AIModule aiModule)
        {
            module = Object.Instantiate(aiModule);
            module.Init();

            foreach (var action in module.runningActions)
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

            if (module != null)
            {
                Object.Destroy(module);
                module = null;
            }
        }

        public void Tick()
        {
            if (!isRunning) return;

            var isTransitioned = false;
            if (isInterruptCheckRequired)
            {
                isInterruptCheckRequired = false;
                isTransitioned = TryTransition(module.interrupts);
                if (isTransitioned)
                {
                    return;
                }
            }

            isTransitioned = TryTransition(currentAction.transitions);
            if (isTransitioned)
            {
                return;
            }

            if (currentAction != null)
            {
                currentAction.Tick();
            }
        }

        private void OnAIMemoryChanged(AIState state, bool value)
        {
            isInterruptCheckRequired = true;
        }

        private bool TryTransition(List<AIAction.Transition> transitions)
        {
            var validTransitions = new List<AIAction.Transition>();
            var priority = -1;
            foreach (var transition in transitions)
            {
                if (!IsValidTransition(transition)) continue;
                
                if (transition.priority > priority)
                {
                    validTransitions = new List<AIAction.Transition> { transition };
                    priority = transition.priority;
                }
                else if (transition.priority == priority)
                {
                    validTransitions.Add(transition);
                }
            }

            if (validTransitions.Any())
            {
                var index = Random.Range(0, validTransitions.Count);
                TransitionToAction(validTransitions[index].actionGuid);
                return true;
            }

            return false;
        }

        private bool IsValidTransition(AIAction.Transition transition)
        {
            if (currentAction != null && transition.actionGuid == currentAction.guid) return false;
            
            var posConditionsMet = transition.pConditions.All(c => agent.memory.IsMatch(c, true));
            if (!posConditionsMet) return false;

            var negConditionsMet = transition.nConditions.All(c => agent.memory.IsMatch(c, false));
            if (!negConditionsMet) return false;
            
            var action = module.runningActions.FirstOrDefault(a => a.guid == transition.actionGuid);
            return action != null && action.CanRun();
        }

        private void TransitionToAction(string actionGuid)
        {
            var action = module.runningActions.FirstOrDefault(a => a.guid == actionGuid);
            if (action == null)
            {
                Debug.LogError($"Failed to find action with ID {actionGuid} in module {module.name}");
                return;
            }

            if (currentAction != null)
            {
                currentAction.Stop();
            }
            
            currentAction = action;
            currentAction.Start();
        }
    }
}