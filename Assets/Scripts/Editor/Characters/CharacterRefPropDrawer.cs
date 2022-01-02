using System;
using System.Linq;
using System.Reflection;
using HollowForest.Data;
using UnityEditor;
using UnityEngine;

namespace HollowForest
{
    [CustomPropertyDrawer(typeof(CharacterRef))]
    public class CharacterRefPropDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var config = Configuration.LoadConfigurationInEditor();

            var guidProperty = property.FindPropertyRelative(nameof(CharacterRef.guid));
            var guid = guidProperty.stringValue;

            var category = GetCategoryAttribute(property);

            var charProfiles = config.characterProfiles.Where(p => p.category == category).ToList();
            if (!charProfiles.Any())
            {
                Debug.LogWarning($"No character profiles found for category {category} but an asset requires it!");
                return;
            }
            
            var selectedProfileIndex = charProfiles.FindIndex(p => p.guid == guid);
            if (selectedProfileIndex < 0)
            {
                selectedProfileIndex = 0;
                SetCharacter(property, charProfiles[selectedProfileIndex]);
            }
            
            var characterNames = charProfiles.Select(p => new GUIContent(p.characterName, p.description)).ToArray();
            var content = new GUIContent(label.text, charProfiles[selectedProfileIndex].description);
            var newIndex = EditorGUI.Popup(position, content, selectedProfileIndex, characterNames);
            if (newIndex != selectedProfileIndex)
            {
                SetCharacter(property, charProfiles[newIndex]);
            }
        }

        private void SetCharacter(SerializedProperty property, CharacterProfile profile)
        {
            var values = ((CharacterCategory[]) Enum.GetValues(typeof(CharacterCategory))).ToList();
            property.FindPropertyRelative(nameof(CharacterRef.guid)).stringValue = profile.guid;
            property.serializedObject.ApplyModifiedProperties();

            property.serializedObject.targetObject.name = profile.characterName + " Spawn";
        }

        private static CharacterCategory GetCategoryAttribute(SerializedProperty property)
        {
            var parentObjType = property.serializedObject.targetObject.GetType();
            var field = parentObjType.GetFields().FirstOrDefault(f => property.name == f.Name);
            if (field != null)
            {
                var attr = field.GetCustomAttribute<CharacterCategoryAttribute>();
                if (attr != null)
                {
                    return attr.category;
                }
            }
            

            return CharacterCategory.None;
        }
    }
}