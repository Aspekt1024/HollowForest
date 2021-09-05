using System;
using System.Collections.Generic;
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

        private VisualElement content;
        
        public DialogueNode(DialoguePage dialoguePage, DialogueConfig.Conversation conversation, int index) : base(new Guid(conversation.dialogueGuid), DialogueNodeProfiles.DependencyProfiles)
        {   
            this.dialoguePage = dialoguePage;
            this.conversation = conversation;
            conversationIndex = index;
            
            AddContextMenuItem("Create Link", pos => dialoguePage.BeginLinkCreation(this, DialogueNodeProfiles.LinkDialogueDependency));
            AddContextMenuItem("Remove Link", pos => dialoguePage.BeginLinkRemoval(this, DialogueNodeProfiles.LinkDialogueDependency));
            AddContextMenuItem("Delete", pos => dialoguePage.RemoveConversation(conversation));
        }

        public void RecordUndo(string message) => dialoguePage.RecordDialogueUndo(message);
        public void Refresh() => Populate(content);
        
        public bool IsLinkedTo(DialogueNode node) => node.conversation.linkedConversations.Contains(conversation.dialogueGuid);
        
        public override bool CreateDependency(Node linkingNode)
        {
            if (!(linkingNode is DialogueNode linkingDialogue)) return false;
            if (linkingDialogue == this) return false;
            
            if (linkingDialogue.conversation.linkedConversations.Contains(conversation.dialogueGuid)) return false;
            linkingDialogue.conversation.linkedConversations.Add(conversation.dialogueGuid);
            return true;
        }

        public override bool RemoveDependency(Node linkingNode)
        {
            if (!(linkingNode is DialogueNode linkingDialogue)) return false;
            
            if (!linkingDialogue.conversation.linkedConversations.Contains(conversation.dialogueGuid)) return false;
            linkingDialogue.conversation.linkedConversations.Remove(conversation.dialogueGuid);
            return true;
        }

        protected override void Populate(VisualElement content)
        {
            this.content = content;
            content.Clear();
            content.AddToClassList("dialogue-node");

            SetStyle(conversation.isOneTime ? DialogueNodeProfiles.OnetimeStyle : DialogueNodeProfiles.StandardStyle);
            SetSelectedState();

            SetSize(new Vector2(200, 120));

            SetupTopSection();

            CreateDialogueDisplay(content);
        }

        private void SetupTopSection()
        {
            var topSection = new VisualElement();
            topSection.AddToClassList("top-section");
            if (conversation.isOneTime)
            {
                topSection.AddToClassList("top-section-onetime");
            }
            if (conversationIndex == 0)
            {
                topSection.AddToClassList("top-section-start-node");
                var label = new Label("Start");
                label.AddToClassList("top-section-start-node-label");
                topSection.Add(label);
            }
            content.Add(topSection);
        }

        private void CreateDialogueDisplay(VisualElement content)
        {
            var dialogueDisplay = new VisualElement();
            for (int i = 0; i < conversation.dialogueLines.Count; i++)
            {
                dialogueDisplay.Add(CreateDialogueLineDisplay(content, conversation.dialogueLines[i], i));
            }

            var addLineButton = new Button {text = "Add line"};
            addLineButton.clicked += () =>
            {
                dialoguePage.RecordDialogueUndo("Add dialogue line");
                conversation.dialogueLines.Add("");
                Populate(content);
            };
            dialogueDisplay.Add(addLineButton);
            content.Add(dialogueDisplay);
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