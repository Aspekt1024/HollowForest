using System;
using Aspekt.Editors;
using HollowForest.Dialogue.Pages;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    [Serializable]
    public class DialogueNode : Node
    {
        private readonly DialogueConfig.Conversation conversation;
        
        public DialogueNode(NodePage nodePage, DialogueConfig.Conversation conversation) : base(new Guid(conversation.dialogueGuid))
        {
            this.conversation = conversation;
            
            AddContextMenuItem("Remove item", mousePos => nodePage.RemoveConversation(conversation));
        }

        protected override void Populate(VisualElement element)
        {
            SetSize(new Vector2(200, 120));
            element.AddToClassList("dialogue-node");

            element.Add(new Label(conversation.dialogueGuid));
            foreach (var dialogueLine in conversation.dialogueLines)
            {
                element.Add(new Label("> " + dialogueLine));
            }
        }
    }
}