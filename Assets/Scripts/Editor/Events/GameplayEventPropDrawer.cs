using System.Linq;
using HollowForest.Data;
using HollowForest.Events;
using UnityEditor;
using UnityEngine;

namespace HollowForest.Objects
{
    [CustomPropertyDrawer(typeof(GameplayEvent))]
    public class GameplayEventPropDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var config = Configuration.LoadConfigurationInEditor();
            var eventRef = new GameplayEvent(); // Used for nameof() references to properties so we don't have to use strings

            var idProperty = property.FindPropertyRelative(nameof(eventRef.eventID));
            var id = idProperty.intValue;
            var selectedEventIndex = config.events.FindIndex(e => e.eventID == id);
            if (selectedEventIndex < 0)
            {
                selectedEventIndex = 0;
                SetGameplayEvent(idProperty, config.events[selectedEventIndex]);
            }
            
            var eventNames = config.events.Select(e => new GUIContent(e.eventName, e.description)).ToArray();
            var content = new GUIContent(label.text, config.events[selectedEventIndex].description);
            var newIndex = EditorGUI.Popup(position, content, selectedEventIndex, eventNames);
            if (newIndex != selectedEventIndex)
            {
                SetGameplayEvent(property, config.events[newIndex]);
            }
        }

        private void SetGameplayEvent(SerializedProperty property, GameplayEvent gameplayEvent)
        {
            var eventRef = new GameplayEvent();
            
            property.FindPropertyRelative(nameof(eventRef.eventID)).intValue = gameplayEvent.eventID;
            property.FindPropertyRelative(nameof(eventRef.eventName)).stringValue = gameplayEvent.eventName;
            property.FindPropertyRelative(nameof(eventRef.description)).stringValue = gameplayEvent.description;
            
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}