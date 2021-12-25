using System;
using Aspekt.Editors;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class InterruptNode : Node
    {
        private readonly ModulePage modulePage;
        
        public InterruptNode(ModulePage modulePage, string nodeGuid) : base(new Guid(nodeGuid), AINodeProfiles.DependencyProfiles)
        {
            this.modulePage = modulePage;
            
            AddContextMenuItem("Add Interrupt", pos => modulePage.BeginLinkCreation(this, AINodeProfiles.InterruptTransition));
            AddContextMenuItem("Remove Interrupt", pos => modulePage.BeginLinkRemoval(this, AINodeProfiles.InterruptTransition));
        }

        protected override void Populate(VisualElement element)
        {
            element.Clear();
            element.AddToClassList("event-node");
            element.AddToClassList("interrupt-node");
            
            SetStyle(AINodeProfiles.StandardStyle);
            SetSize(new Vector2(80, 40));

            var label = new Label("Interrupts");
            label.AddToClassList("event-node-label");
            element.Add(label);
        }
    }
}