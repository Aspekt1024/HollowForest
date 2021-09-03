using System;
using System.Collections.Generic;
using System.Linq;
using Aspekt.Editors;
using HollowForest.Dialogue.Pages;
using UnityEditor;
using UnityEditor.UIElements;
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
            activatingLink = "dialogue-node-activating-link",
        };

        public static int LinkDialogueDependency = 1000;
        public static readonly List<DependencyProfile> DependencyProfiles = new List<DependencyProfile>
        {
            new DependencyProfile(LinkDialogueDependency, Color.cyan) { lineThickness = 1.5f }
        };
        
        public DialogueNode(DialoguePage dialoguePage, DialogueConfig.Conversation conversation, int index) : base(new Guid(conversation.dialogueGuid), Styles, DependencyProfiles)
        {   
            this.dialoguePage = dialoguePage;
            this.conversation = conversation;
            conversationIndex = index;
            
            AddContextMenuItem("Create Link", pos => dialoguePage.BeginLinkCreation(this, LinkDialogueDependency));
            AddContextMenuItem("Remove Link", pos => dialoguePage.BeginLinkRemoval(this, LinkDialogueDependency));
            AddContextMenuItem("Delete", pos => dialoguePage.RemoveConversation(conversation));
        }
        
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
            content.Clear();
            
            SetSize(new Vector2(200, 120));
            content.AddToClassList("dialogue-node");

            if (conversationIndex == 0)
            {
                content.AddToClassList("first-conversation");
            }

            CreateEventsDisplay(content);
            CreateDialogueDisplay(content);

        }

        public bool IsLinkedFrom(DialogueNode node) => node.conversation.linkedConversations.Contains(conversation.dialogueGuid);

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

        private void CreateEventsDisplay(VisualElement content)
        {
            var eventsDisplay = new VisualElement();

            var events = dialoguePage.Editor.Config.events;
            var eventIndexes = new List<int>();
            for (int i = 0; i < events.Count; i++)
            {
                eventIndexes.Add(i);
            }

            for (int i = 0; i < conversation.requiredEvents.Count; i++)
            {
                var requriedEventIndex = i;
                var requiredEvent = conversation.requiredEvents[i];
                var index = events.FindIndex(e => e.eventID == requiredEvent);
                if (index < 0)
                {
                    index = 0;
                    requiredEvent = events[index].eventID;
                    ModifyRequiredEvent(i, requiredEvent);
                }

                var eventPopup = new PopupField<int>(eventIndexes, index,
                    eventIndex => events[eventIndex].eventName,
                    eventIndex => events[eventIndex].eventName
                );
                eventPopup.tooltip = events[index].description;
                eventPopup.RegisterValueChangedCallback(e => ModifyRequiredEvent(requriedEventIndex, events[e.newValue].eventID));

                var removeEventButton = new Button {text = "x"};
                removeEventButton.clicked += () =>
                {
                    dialoguePage.RecordDialogueUndo("Remove required event");
                    conversation.requiredEvents.RemoveAt(requriedEventIndex);
                    Populate(content);
                };
                removeEventButton.AddToClassList("dialogue-button");
                
                var eventItem = new VisualElement();
                eventItem.AddToClassList("dialogue-event-item");
                
                eventItem.Add(eventPopup);
                eventItem.Add(removeEventButton);
                
                eventsDisplay.Add(eventItem);
            }

            var addEventButton = new Button {text = "Add Required Event"};
            addEventButton.clicked += () =>
            {
                dialoguePage.RecordDialogueUndo("Add required event");
                conversation.requiredEvents.Add(events[0].eventID);
                Populate(content);
            };
            
            eventsDisplay.Add(addEventButton);
                
            content.Add(eventsDisplay);
        }

        private void ModifyRequiredEvent(int index, int id)
        {
            dialoguePage.RecordDialogueUndo("Modify required event");
            conversation.requiredEvents[index] = id;
        }
    }
}