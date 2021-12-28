using System;
using Aspekt.Editors;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class InterruptNode : Node
    {
        private readonly ModuleBasePage modulePage;
        
        public InterruptNode(ModuleBasePage modulePage, string nodeGuid) : base(new Guid(nodeGuid), AINodeProfiles.DependencyProfiles)
        {
            this.modulePage = modulePage;

            if (modulePage.CanEdit)
            {
                AddContextMenuItem("Add Interrupt", pos => modulePage.BeginLinkCreation(this, AINodeProfiles.InterruptTransition));
                AddContextMenuItem("Remove Interrupt", pos => modulePage.BeginLinkRemoval(this, AINodeProfiles.InterruptTransition));
            }
        }

        protected override void Populate(VisualElement element)
        {
            element.Clear();
            element.AddToClassList("event-node");
            element.AddToClassList("interrupt-node");
            
            SetStyle(AINodeProfiles.InterruptNodeStyle);
            SetSize(new Vector2(80, 40));

            var label = new Label("Interrupts");
            label.AddToClassList("event-node-label");
            element.Add(label);
        }

        public override void PopulateInspector(VisualElement container)
        {
            container.Clear();
            
            var header = new Label("Interrupts");
            header.AddToClassList("inspector-header");
            container.Add(header);
            
            var module = modulePage.Module;
            foreach (var interrupt in module.interrupts)
            {
                container.Add(TransitionDisplay.Create(interrupt, module, (action, msg) =>
                {
                    modulePage.RecordModuleUndo(msg);
                    action.Invoke();
                    PopulateInspector(container);
                    modulePage.UpdateContents();
                }));
            }
        }
    }
}