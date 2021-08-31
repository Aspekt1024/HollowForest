using System;
using Aspekt.Editors;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    [Serializable]
    public class DialogueNode : Node
    {
        public DialogueNode(Guid guid) : base(guid)
        {
        }

        protected override void Populate(VisualElement element)
        {
            SetSize(new Vector2(100, 40));
            
            element.Add(new Label("dialogueNode"));
        }
    }
}