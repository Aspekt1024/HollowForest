using System.Collections.Generic;
using HollowForest.Data;
using HollowForest.Events;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    public static class ConversationInspector
    {
        public static VisualElement GetInspector(DialogueNode dialogueNode, DialogueConfig.ConversationSet set, Configuration config)
        {
            var inspector = new VisualElement();
            inspector.AddToClassList("inspector");
            
            var index = set.conversations.FindIndex(c => c.dialogueGuid == dialogueNode.serializableGuid);
            if (index < 0) return inspector;

            var conversation = set.conversations[index];
            
            CreateToggle(inspector, dialogueNode, conversation);
            CreateEvents(inspector, dialogueNode, conversation, config);

            return inspector;
        }

        private static void CreateToggle(VisualElement inspector, DialogueNode dialogueNode, DialogueConfig.Conversation conversation)
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

        private struct EventSettings
        {
            public string type;
            public string styleClass;
            public List<int> nodeEvents;
            public DialogueNode node;
            public List<GameplayEvent> allEvents;
        }
        
        private static void CreateEvents(VisualElement inspector, DialogueNode dialogueNode, DialogueConfig.Conversation conversation, Configuration config)
        {
            var requiredEventsElement = new VisualElement();
            var requiredEventSettings = new EventSettings
            {
                type = "Required Event",
                styleClass = "eventlist-required",
                nodeEvents = conversation.requiredEvents,
                node = dialogueNode,
                allEvents = config.events,
            };
            CreateEventsDisplay(requiredEventsElement, requiredEventSettings);
            
            var invalidatingEventsElement = new VisualElement();
            var invalidatingEventSettings = new EventSettings
            {
                type = "Invalidating Event",
                styleClass = "eventlist-invalidating",
                nodeEvents = conversation.invalidatingEvents,
                node = dialogueNode,
                allEvents = config.events,
            };
            CreateEventsDisplay(invalidatingEventsElement, invalidatingEventSettings);
            
            inspector.Add(requiredEventsElement);
            inspector.Add(invalidatingEventsElement);
        }
        
        private static void CreateEventsDisplay(VisualElement eventsDisplay, EventSettings settings)
        {
            eventsDisplay.Clear();
            eventsDisplay.AddToClassList(settings.styleClass);
            
            eventsDisplay.Add(new Label($"{settings.type}s:"));

            var eventIndexes = new List<int>();
            for (int i = 0; i < settings.allEvents.Count; i++)
            {
                eventIndexes.Add(i);
            }

            for (int i = 0; i < settings.nodeEvents.Count; i++)
            {
                var requriedEventIndex = i;
                var requiredEvent = settings.nodeEvents[i];
                var index = settings.allEvents.FindIndex(e => e.eventID == requiredEvent);
                if (index < 0)
                {
                    index = 0;
                    requiredEvent = settings.allEvents[index].eventID;
                    settings.node.RecordUndo($"Modify {settings.type}");
                    settings.nodeEvents[index] = requiredEvent;
                }

                var eventPopup = new PopupField<int>(eventIndexes, index,
                    eventIndex => settings.allEvents[eventIndex].eventName,
                    eventIndex => settings.allEvents[eventIndex].eventName
                );
                eventPopup.tooltip = settings.allEvents[index].description;
                eventPopup.RegisterValueChangedCallback(e =>
                {
                    settings.node.RecordUndo($"Modify {settings.type}");
                    settings.nodeEvents[requriedEventIndex] = settings.allEvents[e.newValue].eventID;
                });

                var removeEventButton = new Button {text = "x"};
                removeEventButton.clicked += () =>
                {
                    settings.node.RecordUndo($"Remove {settings.type}");
                    settings.nodeEvents.RemoveAt(requriedEventIndex);
                    settings.node.Refresh();
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
                settings.node.RecordUndo($"Add {settings.type}");
                settings.nodeEvents.Add(settings.allEvents[0].eventID);
                settings.node.Refresh();
                CreateEventsDisplay(eventsDisplay, settings);
            };
            
            eventsDisplay.Add(addEventButton);
        }
    }
}