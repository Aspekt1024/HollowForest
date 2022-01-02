using System;
using System.Linq;
using Aspekt.Editors;
using HollowForest.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class CharacterPage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Characters";

        private VisualElement charactersContainer;
        
        public CharacterPage(DialogueEditor editor) : base(editor)
        {
        }
        
        public override void UpdateContents()
        {
            charactersContainer.Clear();
            
            if (Editor.Config == null || Editor.Config.characterProfiles == null) return;
            
            var serializedObject = new SerializedObject(Editor.Config);
            var profilesProp = serializedObject.FindProperty(nameof(Configuration.characterProfiles));
            for (int i = 0; i < profilesProp.arraySize; i++)
            {
                var profileProp = profilesProp.GetArrayElementAtIndex(i);
                var profile = Editor.Config.characterProfiles[i];
                charactersContainer.Add(CreateProfileDisplay(profile, profileProp));
            }

            var createButton = new Button() {text = "Create New Character"};
            createButton.clicked += CreateCharacterProfile;
            
            charactersContainer.Add(createButton);
        }

        protected override void SetupUI(VisualElement root)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Profiles.uss");
            root.styleSheets.Add(styleSheet);
            
            charactersContainer = new VisualElement();
            root.Add(charactersContainer);
             
            UpdateContents();
        }

        private VisualElement CreateProfileDisplay(CharacterProfile profile, SerializedProperty profileProp)
        {
            var profileElement = new VisualElement();
            profileElement.AddToClassList("profile");
            
            var profileContents = new VisualElement();
            profileContents.AddToClassList("profile-contents");

            var profileDetails = new VisualElement();
            profileDetails.AddToClassList("profile-details");
            
            var profileName = new TextField() {value = profile.characterName};
            profileName.AddToClassList("profile-name");
            profileName.RegisterValueChangedCallback(e => ProfileNameUpdated(profile, e.newValue));
            profileDetails.Add(profileName);
            
            var profileDescription = new TextField() {value = profile.description};
            profileDescription.AddToClassList("profile-description");
            profileDescription.RegisterValueChangedCallback(e => ProfileDescriptionUpdated(profile, e.newValue));
            profileDetails.Add(profileDescription);

            var profileCategory = new EnumField(profile.category);
            profileCategory.AddToClassList("profile-category");
            profileCategory.RegisterValueChangedCallback(e => ProfileCategoryUpdated(profile, e.newValue));
            profileDetails.Add(profileCategory);

            var assetProp = profileProp.FindPropertyRelative(nameof(profile.asset));
            var assetField = new PropertyField(assetProp, " ");
            assetField.AddToClassList("profile-asset");
            assetField.Bind(assetProp.serializedObject);
            profileDetails.Add(assetField);
            
            profileContents.Add(profileDetails);
            profileElement.Add(profileContents);

            var removeButton = new Button() {text = "Remove"};
            removeButton.AddToClassList("profile-button");
            removeButton.clicked += () => RemoveProfile(profile);
            profileElement.Add(removeButton);

            return profileElement;
        }

        private void ProfileNameUpdated(CharacterProfile profile, string newName)
        {
            Editor.RecordUndo(Editor.Config, "Change character name");
            profile.characterName = newName;
        }

        private void ProfileDescriptionUpdated(CharacterProfile profile, string newDescription)
        {
            Editor.RecordUndo(Editor.Config, "Change character description");
            profile.description = newDescription;
        }

        private void ProfileCategoryUpdated(CharacterProfile profile, Enum newCategory)
        {
            var category = (CharacterCategory) newCategory;
            Editor.RecordUndo(Editor.Config, "Change character category");
            profile.category = category;
        }

        private void CreateCharacterProfile()
        {
            Editor.RecordUndo(Editor.Config, "Add character profile");

            var profile = new CharacterProfile
            {
                guid = Guid.NewGuid().ToString(),
                characterName = "New Character",
                description = "",
            };
            Editor.Config.characterProfiles.Add(profile);
            
            UpdateContents();
        }

        private void RemoveProfile(CharacterProfile profile)
        {
            Editor.RecordUndo(Editor.Config, $"Remove character profile");
            Editor.Config.characterProfiles.Remove(profile);
            UpdateContents();
        }
    }
}