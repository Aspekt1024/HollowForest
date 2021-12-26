using System;
using Aspekt.Editors;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class EntryNode : Node
    {
        public EntryNode(string nodeGuid) : base(new Guid(nodeGuid), AINodeProfiles.DependencyProfiles)
        {
        }

        protected override void Populate(VisualElement element)
        {
            element.Clear();
            element.AddToClassList("event-node");
            element.AddToClassList("entry-node");
            
            SetStyle(AINodeProfiles.EntryNodeStyle);
            SetSize(new Vector2(60, 40));

            var label = new Label("Entry");
            label.AddToClassList("event-node-label");
            element.Add(label);
        }
    }
}