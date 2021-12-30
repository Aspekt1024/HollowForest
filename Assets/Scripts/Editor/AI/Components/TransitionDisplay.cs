using System;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public static class TransitionDisplay
    {
        public static VisualElement Create(AIAction.Transition transition, AIModule module, Action<Action, string> updateCallback)
        {
            var display = new VisualElement();
            display.AddToClassList("transition-display");

            var action = module.actions.FirstOrDefault(a => a.guid == transition.actionGuid);
            if (action == null)
            {
                display.Add(new Label("invalid action"));
            }
            else
            {
                display.Add(new Label($"{action.DisplayName} ({action.guid.Substring(0, 8)}...)"));
            }

            var requireActionCompleteToggle = new Toggle("Requires Action Completion")
            {
                value = transition.requiresActionCompletion,
            };
            requireActionCompleteToggle.RegisterValueChangedCallback(e =>
            {
                updateCallback?.Invoke(
                    () => transition.requiresActionCompletion = e.newValue,
                    "Modify action completion condition"
                );
            });
            display.Add(requireActionCompleteToggle);

            var numConditions = transition.nConditions.Count + transition.pConditions.Count;
            if (numConditions > 0)
            {
                display.Add(new Label("Conditions:"));
                for (int i = 0; i < transition.pConditions.Count; i++)
                {
                    display.Add(CreateConditionDisplay(transition, i, true, updateCallback));
                }
                for (int i = 0; i < transition.nConditions.Count; i++)
                {
                    display.Add(CreateConditionDisplay(transition, i, false, updateCallback));
                }
            }
            else
            {
                display.Add(new Label("No transition conditions exist."));
            }

            var createConditionButton = new Button { text = "Create Condition" };
            createConditionButton.clicked += () =>
            {
                updateCallback?.Invoke(
                    () => transition.pConditions.Add(((AIState[])Enum.GetValues(typeof(AIState)))[0]),
                    "Add transition condition"
                );
            };
            display.Add(createConditionButton);
            
            // TODO remove transition button
            
            return display;
        }

        private static VisualElement CreateConditionDisplay(AIAction.Transition transition, int index, bool isTrue, Action<Action, string> updateCallback)
        {
            var state = isTrue ? transition.pConditions[index] : transition.nConditions[index];
            state = ValidateState(state);
            
            var container = new VisualElement();
            container.AddToClassList("transition-condition");

            var states = ((AIState[])Enum.GetValues(typeof(AIState))).ToList();
            var conditionPopup = new PopupField<AIState>(states, state,
                newState =>
                {
                    if (state == newState) return state.ToString();
                    updateCallback.Invoke(() =>
                    {
                        if (isTrue)
                        {
                            transition.pConditions[index] = newState;
                        }
                        else
                        {
                            transition.nConditions[index] = newState;
                        }
                    },
                    "Modify transition condition");
                    return newState.ToString();
                },
                s => s.ToString()
            );
            conditionPopup.AddToClassList(isTrue ? "transition-condition-positive" : "transition-condition-negative");
            container.Add(conditionPopup);

            var toggle = new Toggle { value = isTrue };
            toggle.RegisterValueChangedCallback(e =>
            {
                updateCallback.Invoke(() => {
                    if (isTrue)
                    {
                        transition.pConditions.RemoveAt(index);
                        transition.nConditions.Add(state);
                    }
                    else
                    {
                        transition.nConditions.RemoveAt(index);
                        transition.pConditions.Add(state);
                    }},
                    "Modify transition condition"
                );
            });
            container.Add(toggle);

            var removeButton = new Button { text = "X" };
            removeButton.clicked += () =>
            {
                updateCallback.Invoke(() =>
                {
                    if (isTrue)
                    {
                        transition.pConditions.RemoveAt(index);
                    }
                    else
                    {
                        transition.nConditions.RemoveAt(index);
                    }
                },
                "Remove transition condition");
            };
            container.Add(removeButton);
            
            return container;
        }

        private static AIState ValidateState(AIState state)
        {
            var success = Enum.IsDefined(typeof(AIState), state);
            if (!success)
            {
                state = ((AIState[])Enum.GetValues(typeof(AIState)))[0];
            }

            return state;
        }
    }
}