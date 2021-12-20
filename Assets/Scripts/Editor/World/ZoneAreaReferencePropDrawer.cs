using System.Collections.Generic;
using System.Linq;
using HollowForest.Data;
using HollowForest.Events;
using UnityEditor;
using UnityEngine;

namespace HollowForest.World
{
    [CustomPropertyDrawer(typeof(ZoneAreaReference))]
    public class ZoneAreaReferencePropDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var config = Configuration.LoadConfigurationInEditor();

            var idProperty = property.FindPropertyRelative(nameof(ZoneAreaReference.zoneAreaID));
            var id = idProperty.intValue;
            
            var zoneAreaReferences = new List<ZoneAreaReference> {new ZoneAreaReference {zoneAreaID = 0, zoneAreaName = "None", description = "No zone area"}};
            zoneAreaReferences.AddRange(config.zoneAreaReferences);
            
            var selectedAreaRef = zoneAreaReferences.FindIndex(r => r.zoneAreaID == id);
            if (selectedAreaRef < 0)
            {
                selectedAreaRef = 0;
                SetZoneAreaReference(property, zoneAreaReferences[selectedAreaRef]);
            }

            var zoneAreaRefNames = zoneAreaReferences.Select(e => new GUIContent(e.zoneAreaName, e.description)).ToArray();
            var content = new GUIContent(label.text, zoneAreaReferences[selectedAreaRef].description);
            var newIndex = EditorGUI.Popup(position, content, selectedAreaRef, zoneAreaRefNames);
            if (newIndex != selectedAreaRef)
            {
                SetZoneAreaReference(property, zoneAreaReferences[newIndex]);
            }
        }

        private void SetZoneAreaReference(SerializedProperty property, ZoneAreaReference zoneAreaReference)
        {
            property.FindPropertyRelative(nameof(ZoneAreaReference.zoneAreaID)).intValue = zoneAreaReference.zoneAreaID;
            property.FindPropertyRelative(nameof(ZoneAreaReference.zoneAreaName)).stringValue = zoneAreaReference.zoneAreaName;
            property.FindPropertyRelative(nameof(ZoneAreaReference.description)).stringValue = zoneAreaReference.description;
            
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}