using System;
using Aspekt.Editors;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    [Serializable]
    public class DialogueNode : Node
    {
        private readonly DialogueConfig.Conversation conversation;
        
        public DialogueNode(DialogueConfig.Conversation conversation) : base(new Guid(conversation.conversationGuid))
        {
            this.conversation = conversation;
        }

        protected override void Populate(VisualElement element)
        {
            SetSize(new Vector2(200, 120));
            element.AddToClassList("dialogue-node");

            foreach (var dialogueLine in conversation.dialogueLines)
            {
                element.Add(new Label("> " + dialogueLine));
            }
        }
    }
}