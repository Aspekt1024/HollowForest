using System.Collections.Generic;
using System.Linq;
using HollowForest.Data;
using HollowForest.Objects;
using UnityEditor;
using UnityEngine;

namespace HollowForest.World
{
    [CustomPropertyDrawer(typeof(ItemRef))]
    public class ItemRefPropDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var config = Configuration.LoadConfigurationInEditor();

            var idProperty = property.FindPropertyRelative(nameof(ItemRef.id));
            var id = idProperty.intValue;
            
            var items = new List<Item> {new Item {id = 0, name = "None", description = "No item"}};
            items.AddRange(config.items);
            
            var selectedItemIndex = items.FindIndex(item => item.id == id);
            if (selectedItemIndex < 0)
            {
                selectedItemIndex = 0;
                SetItemReference(property, items[selectedItemIndex]);
            }

            var itemNames = items.Select(item => new GUIContent(item.name, item.description)).ToArray();
            var content = new GUIContent(label.text, items[selectedItemIndex].description);
            var newIndex = EditorGUI.Popup(position, content, selectedItemIndex, itemNames);
            if (newIndex != selectedItemIndex)
            {
                SetItemReference(property, items[newIndex]);
            }
        }

        private void SetItemReference(SerializedProperty property, Item item)
        {
            property.FindPropertyRelative(nameof(ItemRef.id)).intValue = item.id;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}