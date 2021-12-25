using System;
using System.Collections.Generic;
using System.Linq;
using Aspekt.Editors;
using HollowForest.AI.States;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class ActionNode : Node
    {
        private readonly ModulePage modulePage;
        public readonly AIAction Action;

        private VisualElement element;
        
        public ActionNode(ModulePage modulePage, AIAction action) : base(new Guid(action.guid), AINodeProfiles.DependencyProfiles)
        {
            this.modulePage = modulePage;
            Action = action;
            
            AddContextMenuItem("Create Transition", pos => modulePage.BeginLinkCreation(this, AINodeProfiles.ActionTransition));
            AddContextMenuItem("Remove Transition", pos => modulePage.BeginLinkRemoval(this, AINodeProfiles.ActionTransition));
            AddContextMenuItem("Set Default Action", pos => modulePage.SetDefaultAction(this));
            AddContextMenuItem("Delete", pos => modulePage.RemoveAction(this));
        }

        public bool IsTransitionedFrom(ActionNode node)
        {
            return node.Action.transitions.Any(t => t.actionGuid == Action.guid);
        }

        public override bool CreateDependency(Node dependency)
        {
            if (dependency is ActionNode actionNode)
            {
                if (Action.transitions.Any(t => t.actionGuid == actionNode.Action.guid))
                {
                    return false;
                }

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
            element.AddToClassList("action-node");
            
            SetStyle(AINodeProfiles.StandardStyle);
            SetSize(new Vector2(200, 120));

            SetupTopSection();

            CreateDisplay(element);
        }
        
        private void SetupTopSection()
        {
            var topSection = new VisualElement();
            topSection.AddToClassList("top-section");
            var label = new Label(Action.name);
            topSection.Add(label);
            element.Add(topSection);
        }

        private void CreateDisplay(VisualElement element)
        {
            var actionDisplay = new VisualElement();


            element = actionDisplay;
        }
    }
}