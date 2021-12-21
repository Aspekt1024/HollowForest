using System;
using System.Collections.Generic;
using System.Linq;
using Aspekt.Editors;
using HollowForest.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class EventsPage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Events";

        private readonly VisualElement eventsContainer;
        
        public EventsPage(DialogueEditor editor) : base(editor)
        {
            eventsContainer = new VisualElement();
        }
        
        public override void UpdateContents()
        {
            eventsContainer.Clear();
            
            if (Editor.Config == null || Editor.Config.events == null) return;
            
            var events = Editor.Config.events;
            foreach (var gameplayEvent in events)
            {
                eventsContainer.Add(CreateEventDisplay(gameplayEvent));
            }

            var createButton = new Button() {text = "Create New Event"};
            createButton.clicked += CreateGameplayEvent;
            
            eventsContainer.Add(createButton);
        }

        protected override void SetupUI(VisualElement root)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Events.uss");
            root.styleSheets.Add(styleSheet);
            
            root.Add(eventsContainer);
             
            UpdateContents();
        }

        private VisualElement CreateEventDisplay(GameplayEvent gameplayEvent)
        {
            var eventElement = new VisualElement();
            eventElement.AddToClassList("event");
            
            var eventContents = new VisualElement();
            eventContents.AddToClassList("event-contents");

            var eventDetails = new VisualElement();
            eventDetails.AddToClassList("event-details");
            var id = new Label(gameplayEvent.eventID.ToString());
            id.AddToClassList("event-id");
            eventDetails.Add(id);
            
            var eventName = new TextField() {value = gameplayEvent.eventName};
            eventName.AddToClassList("event-name");
            eventName.RegisterValueChangedCallback(e => EventNameUpdated(gameplayEvent, e.newValue));
            eventDetails.Add(eventName);
            
            var eventDescription = new TextField() {value = gameplayEvent.description};
            eventDescription.AddToClassList("event-description");
            eventDescription.RegisterValueChangedCallback(e => EventDescriptionUpdated(gameplayEvent, e.newValue));
            eventDetails.Add(eventDescription);
            
            eventContents.Add(eventDetails);
            eventElement.Add(eventContents);

            var removeButton = new Button() {text = "Remove"};
            removeButton.AddToClassList("event-button");
            removeButton.clicked += () => RemoveEvent(gameplayEvent);
            eventElement.Add(removeButton);

            return eventElement;
        }

        private void EventNameUpdated(GameplayEvent e, string newName)
        {
            Editor.RecordUndo(Editor.Config, "Change gameplay event name");
            e.eventName = newName;
        }

        private void EventDescriptionUpdated(GameplayEvent e, string newDescription)
        {
            Editor.RecordUndo(Editor.Config, "Change gameplay event description");
            e.description = newDescription;
        }

        private int GetUniqueEventID()
        {
            var id = 10000;
            if (Editor.Config.events.Any())
            {
                id = Editor.Config.events[Editor.Config.events.Count - 1].eventID + 1;
            }
            
            return id;
        }

        private void CreateGameplayEvent()
        {
            Editor.RecordUndo(Editor.Config, "Add gameplay event");
                
            var uniqueEventID = GetUniqueEventID();
            Editor.Config.events.Add(new GameplayEvent
            {
                eventID = uniqueEventID,
                eventName = "Untitled Event",
                description = "",
            });
            
            UpdateContents();
        }

        private void RemoveEvent(GameplayEvent e)
        {
            Editor.RecordUndo(Editor.Config, $"Remove gameplay event {e.eventID}");
            Editor.Config.events.Remove(e);
            UpdateContents();
        }
    }
}