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

        private readonly DialoguePage dialoguePage;
        private readonly int conversationIndex;

        private static readonly StyleProfile Styles = new StyleProfile
        {
            baseStyle = "dialogue-node",
            selectedStyle = "dialogue-node-selected",
            unselectedStyle = "dialogue-node-unselected",
        };
        
        public DialogueNode(DialoguePage dialoguePage, DialogueConfig.Conversation conversation, int index) : base(new Guid(conversation.dialogueGuid), Styles)
        {   
            this.dialoguePage = dialoguePage;
            this.conversation = conversation;
            conversationIndex = index;
            
            AddContextMenuItem("Delete", mousePos => dialoguePage.RemoveConversation(conversation));
        }

        protected override void Populate(VisualElement content)
        {
            content.Clear();
            
            SetSize(new Vector2(200, 120));
            content.AddToClassList("dialogue-node");

            if (conversationIndex == 0)
            {
                content.AddToClassList("first-conversation");
            }
            
            for (int i = 0; i < conversation.dialogueLines.Count; i++)
            {
                content.Add(CreateDialogueLineDisplay(content, conversation.dialogueLines[i], i));
            }

            var addLineButton = new Button {text = "Add line"};
            addLineButton.clicked += () =>
            {
                dialoguePage.RecordDialogueUndo("Add dialogue line");
                conversation.dialogueLines.Add("");
                Populate(content);
            };
            content.Add(addLineButton);
        }

        private VisualElement CreateDialogueLineDisplay(VisualElement content, string text, int lineIndex)
        {
            var dialogueLine = new VisualElement();
            dialogueLine.AddToClassList("dialogue-line");

            var textField = new TextField {value = text, multiline = true};
            textField.AddToClassList("dialogue-line-text");
            textField.RegisterValueChangedCallback(e =>
            {
                dialoguePage.RecordDialogueUndo("Modify dialogue");
                conversation.dialogueLines[lineIndex] = e.newValue;
            });

            dialogueLine.Add(textField);
            
            var removeLineButton = new Button {text = "x"};
            removeLineButton.clicked += () =>
            {
                dialoguePage.RecordDialogueUndo("Remove dialogue line");
                conversation.dialogueLines.RemoveAt(lineIndex);
                Populate(content);
            };
            
            dialogueLine.Add(removeLineButton);
            
            return dialogueLine;
        }
    }
}