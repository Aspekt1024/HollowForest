using System;
using System.Linq;
using Aspekt.Editors;
using UnityEditor;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class CharacterPage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Characters";

        private VisualElement charactersContainer;
        
        public CharacterPage(DialogueEditor editor, VisualElement root) : base(editor, root)
        {
        }
        
        public override void UpdateContents()
        {
            charactersContainer.Clear();
            
            if (Editor.Config == null || Editor.Config.characterProfiles == null) return;
            
            var profiles = Editor.Config.characterProfiles;
            foreach (var profile in profiles)
            {
                charactersContainer.Add(CreateProfileDisplay(profile));
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

        private VisualElement CreateProfileDisplay(CharacterProfile profile)
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
            profileDescription.RegisterValueChangedCallback(e => EventDescriptionUpdated(profile, e.newValue));
            profileDetails.Add(profileDescription);
            
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

        private void EventDescriptionUpdated(CharacterProfile profile, string newDescription)
        {
            Editor.RecordUndo(Editor.Config, "Change character description");
            profile.description = newDescription;
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