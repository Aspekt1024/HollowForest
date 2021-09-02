using System.Linq;
using HollowForest.Data;
using UnityEditor;
using UnityEngine;

namespace HollowForest
{
    [CustomPropertyDrawer(typeof(CharacterProfile))]
    public class CharacterProfilePropDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var config = Configuration.LoadConfigurationInEditor();
            var profileRef = new CharacterProfile(); // Used for nameof() references to properties so we don't have to use strings

            var guidProperty = property.FindPropertyRelative(nameof(profileRef.guid));
            var guid = guidProperty.stringValue;
            
            var selectedProfileIndex = config.characterProfiles.FindIndex(p => p.guid == guid);
            if (selectedProfileIndex < 0)
            {
                selectedProfileIndex = 0;
                SetCharacterProfile(property, config.characterProfiles[selectedProfileIndex]);
            }
            
            var characterNames = config.characterProfiles.Select(p => new GUIContent(p.characterName, p.description)).ToArray();
            var content = new GUIContent(label.text, config.characterProfiles[selectedProfileIndex].description);
            var newIndex = EditorGUI.Popup(position, content, selectedProfileIndex, characterNames);
            if (newIndex != selectedProfileIndex)
            {
                SetCharacterProfile(property, config.characterProfiles[newIndex]);
            }
        }

        private void SetCharacterProfile(SerializedProperty property, CharacterProfile profile)
        {
            var profileRef = new CharacterProfile();
            
            property.FindPropertyRelative(nameof(profileRef.guid)).stringValue = profile.guid;
            property.FindPropertyRelative(nameof(profileRef.characterName)).stringValue = profile.characterName;
            property.FindPropertyRelative(nameof(profileRef.description)).stringValue = profile.description;
            
            property.serializedObject.ApplyModifiedProperties();
        } 
    }
}