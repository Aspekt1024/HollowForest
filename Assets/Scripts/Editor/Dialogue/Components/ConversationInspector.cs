using System.Collections.Generic;
using HollowForest.Data;
using HollowForest.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    public class ConversationInspector
    {
        private struct EventSettings
        {
            public string type;
            public string styleClass;
            public List<int> nodeEvents;
        }

        private DialogueNode dialogueNode;
        private DialogueConfig.ConversationSet set;
        private DialogueConfig.Conversation conversation;
        
        private readonly Configuration config;
        private readonly VisualElement inspector;

        public ConversationInspector(VisualElement inspector, Configuration config)
        {
            this.inspector = inspector;
            this.config = config;
            inspector.AddToClassList("inspector");
        }
        
        public void PopulateConversationInfo(DialogueNode dialogueNode, DialogueConfig.ConversationSet set)
        {
            inspector.Clear();
            
            this.dialogueNode = dialogueNode;
            this.set = set;
            
            var index = set.conversations.FindIndex(c => c.dialogueGuid == dialogueNode.serializableGuid);
            if (index < 0) return;

            conversation = set.conversations[index];

            CreateCharacterDropdown(inspector);
            CreateToggle(inspector);
            CreateEvents(inspector);
        }

        public void Clear()
        {
            inspector.Clear();
        }

        private void CreateToggle(VisualElement inspector)
        {
            var toggleElement = new VisualElement();
            toggleElement.AddToClassList("inspector-field");
            var oneTimeToggle = new Toggle() {value = conversation.isOneTime};
            oneTimeToggle.RegisterValueChangedCallback(e =>
            {
                dialogueNode.RecordUndo("Modify one time toggle");
                conversation.isOneTime = !conversation.isOneTime;
                dialogueNode.Refresh();
            });
            
            toggleElement.Add(new Label("Is non-repeating dialogue event:"));
            toggleElement.Add(oneTimeToggle);
            inspector.Add(toggleElement);
        }
        
        private void CreateEvents(VisualElement inspector)
        {
            var requiredEventsElement = new VisualElement();
            var requiredEventSettings = new EventSettings
            {
                type = "Required Event",
                styleClass = "eventlist-required",
                nodeEvents = conversation.requiredEvents,
            };
            CreateEventsDisplay(requiredEventsElement, requiredEventSettings);
            
            var invalidatingEventsElement = new VisualElement();
            var invalidatingEventSettings = new EventSettings
            {
                type = "Invalidating Event",
                styleClass = "eventlist-invalidating",
                nodeEvents = conversation.invalidatingEvents,
            };
            CreateEventsDisplay(invalidatingEventsElement, invalidatingEventSettings);
            
            inspector.Add(requiredEventsElement);
            inspector.Add(invalidatingEventsElement);
        }
        
        private void CreateEventsDisplay(VisualElement eventsDisplay, EventSettings settings)
        {
            eventsDisplay.Clear();
            eventsDisplay.AddToClassList(settings.styleClass);
            
            eventsDisplay.Add(new Label($"{settings.type}s:"));

            var eventIndexes = new List<int>();
            for (int i = 0; i < config.events.Count; i++)
            {
                eventIndexes.Add(i);
            }

            for (int i = 0; i < settings.nodeEvents.Count; i++)
            {
                var requriedEventIndex = i;
                var requiredEvent = settings.nodeEvents[i];
                var index = config.events.FindIndex(e => e.eventID == requiredEvent);
                if (index < 0)
                {
                    index = 0;
                    requiredEvent = config.events[index].eventID;
                    dialogueNode.RecordUndo($"Modify {settings.type}");
                    settings.nodeEvents[index] = requiredEvent;
                }

                var eventPopup = new PopupField<int>(eventIndexes, index,
                    eventIndex => config.events[eventIndex].eventName,
                    eventIndex => config.events[eventIndex].eventName
                );
                eventPopup.tooltip = config.events[index].description;
                eventPopup.RegisterValueChangedCallback(e =>
                {
                    dialogueNode.RecordUndo($"Modify {settings.type}");
                    settings.nodeEvents[requriedEventIndex] = config.events[e.newValue].eventID;
                });

                var removeEventButton = new Button {text = "x"};
                removeEventButton.clicked += () =>
                {
                    dialogueNode.RecordUndo($"Remove {settings.type}");
                    settings.nodeEvents.RemoveAt(requriedEventIndex);
                    dialogueNode.Refresh();
                    CreateEventsDisplay(eventsDisplay, settings);
                };
                removeEventButton.AddToClassList("dialogue-button");
                
                var eventItem = new VisualElement();
                eventItem.AddToClassList("dialogue-event-item");
                
                eventItem.Add(eventPopup);
                eventItem.Add(removeEventButton);
                
                eventsDisplay.Add(eventItem);
            }

            var addEventButton = new Button {text = $"Add {settings.type}"};
            addEventButton.clicked += () =>
            {
                dialogueNode.RecordUndo($"Add {settings.type}");
                settings.nodeEvents.Add(config.events[0].eventID);
                dialogueNode.Refresh();
                CreateEventsDisplay(eventsDisplay, settings);
            };
            
            eventsDisplay.Add(addEventButton);
        }

        private void CreateCharacterDropdown(VisualElement inspector)
        {
            var conversationProp = GetSerializedConversationProperty();
            if (conversationProp == null)
            {
                inspector.Add(new Label("Error: failed to get conversation property"));
                return;
            }
            
            var characterField = new PropertyField(conversationProp.FindPropertyRelative(nameof(conversation.character)));
            characterField.Bind(conversationProp.serializedObject);
            
            inspector.Add(characterField);
        }

        private SerializedProperty GetSerializedConversationProperty()
        {
            var setIndex = config.dialogue.ConversationSets.FindIndex(s => s == set);
            if (setIndex < 0)
            {
                return null;
            }
            
            var conversationindex = config.dialogue.ConversationSets[setIndex].conversations.FindIndex(c => c.dialogueGuid == dialogueNode.serializableGuid);
            if (conversationindex < 0)
            {
                return null;
            }
            
            var serializedObject = new SerializedObject(config.dialogue);
            var setsProp = serializedObject.FindProperty(nameof(config.dialogue.ConversationSets));
            var setProp = setsProp.GetArrayElementAtIndex(setIndex);
            var conversationsProp = setProp.FindPropertyRelative(nameof(set.conversations));
            var conversationProp = conversationsProp.GetArrayElementAtIndex(conversationindex);

            return conversationProp;
        }
    }
}