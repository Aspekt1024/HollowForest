using System;
using System.Linq;
using Aspekt.Editors;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class ActionNode : Node
    {
        private readonly ModuleBasePage modulePage;
        public readonly AIAction Action;

        private VisualElement element;
        
        public ActionNode(ModuleBasePage modulePage, AIAction action) : base(new Guid(action.guid), AINodeProfiles.DependencyProfiles)
        {
            this.modulePage = modulePage;
            Action = action;

            Action.Started += OnActionStarted;
            Action.Stopped += OnActionStopped;

            if (modulePage.CanEdit)
            {
                AddContextMenuItem("Create Transition", pos => modulePage.BeginLinkCreation(this, AINodeProfiles.ActionTransition));
                AddContextMenuItem("Remove Transition", pos => modulePage.BeginLinkRemoval(this, AINodeProfiles.ActionTransition));
                AddContextMenuItem("Set Default Action", pos => modulePage.SetDefaultAction(this));
                AddContextMenuItem("Delete", pos => modulePage.RemoveAction(this));
            }
        }

        public bool IsTransitionedFrom(ActionNode node)
        {
            return node.Action.transitions.Any(t => t.actionGuid == Action.guid);
        }

        public override bool CreateDependency(Node dependency)
        {
            if (dependency is ActionNode actionNode)
            {
                actionNode.Action.transitions.Add(new AIAction.Transition(Action.guid));
                return true;
            }
            else if (dependency is InterruptNode)
            {
                return modulePage.AddInterrupt(this);
            }

            return false;
        }

        public override bool RemoveDependency(Node dependency)
        {
            if (dependency is ActionNode actionNode)
            {
                var transitionIndex = actionNode.Action.transitions.FindIndex(t => t.actionGuid == Action.guid);
                if (transitionIndex < 0) return false;
                
                actionNode.Action.transitions.RemoveAt(transitionIndex);
                return true;
            }
            else if (dependency is InterruptNode)
            {
                return modulePage.RemoveInterrupt(this);
            }

            return false;
        }

        protected override void Populate(VisualElement element)
        {
            this.element = element;
            element.Clear();

            if (Action.IsRunning)
            {
                OnActionStarted();
            }
            else
            {
                OnActionStopped();
            }
            
            element.AddToClassList("action-node");
            
            SetStyle(AINodeProfiles.StandardStyle);
            SetSize(new Vector2(100, 40));

            var label = new Label(Action.DisplayName);
            label.AddToClassList("action-node-label");
            
            element.Add(label);
        }
        
        public override void PopulateInspector(VisualElement container)
        {
            container.Clear();

            var header = new Label($"{Action.DisplayName} ({Action.guid.Substring(0, 8)}...)");
            header.AddToClassList("inspector-header");
            container.Add(header);
            
            var transitionHeader = new Label("Transitions");
            transitionHeader.AddToClassList("inspector-header");
            container.Add(transitionHeader);

            var module = modulePage.Module;
            foreach (var transition in Action.transitions)
            {
                container.Add(TransitionDisplay.Create(transition, module, (action, msg) =>
                {
                    if (modulePage == null) return;
                    modulePage.RecordModuleUndo(msg);
                    action.Invoke();
                    PopulateInspector(container);
                    modulePage.UpdateContents();
                }));
            }
        }

        ~ActionNode()
        {
            Action.Started -= OnActionStarted;
            Action.Stopped -= OnActionStopped;
        }

        private const string ActionRunningStyle = "action-running";
        
        private void OnActionStarted()
        {
            if (!element.ClassListContains(ActionRunningStyle))
            {
                element.AddToClassList(ActionRunningStyle);
            }
        }

        private void OnActionStopped()
        {
            element.RemoveFromClassList(ActionRunningStyle);
        }
    }
}