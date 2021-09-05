using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    public class SetInspector
    {
        private readonly DialogueSidePanel panel;
        private readonly VisualElement setInfo;
        private readonly VisualElement createNewDisplay;

        private bool isCreatingNew;
        private string newSetName;
        
        public event Action<DialogueConfig.ConversationSet> NewConversationSetSelected = delegate {  };

        public SetInspector(DialogueSidePanel panel, VisualElement setInfo)
        {
            this.panel = panel;
            this.setInfo = setInfo;

            createNewDisplay = new VisualElement();
        }

        public void PopulateSetInfo(DialogueConfig config, DialogueConfig.ConversationSet set)
        {
            setInfo.Clear();
            setInfo.AddToClassList("inspector");
            
            var setDropdown = CreateSetDropdown(config, set);
            setInfo.Add(setDropdown);

            var setDetails = CreateSetDetails(config, set);
            setInfo.Add(setDetails);

            setInfo.Add(createNewDisplay);
            PopulateNewSetDetails(createNewDisplay, config);
        }

        private VisualElement CreateSetDropdown(DialogueConfig config, DialogueConfig.ConversationSet currentSet)
        {
            return new PopupField<DialogueConfig.ConversationSet>(config.ConversationSets, currentSet,
                newSet =>
                {
                    if (newSet == currentSet) return newSet.setName;
                    
                    NewConversationSetSelected?.Invoke(newSet);
                    return newSet.setName;
                }, set =>
                {
                    if (set.setName == "")
                    {
                        set.setName = "Unnamed";
                    }
                    return set.setName;
                });
        }

        private VisualElement CreateSetDetails(DialogueConfig config, DialogueConfig.ConversationSet set)
        {
            var details = new VisualElement();
            
            var index = config.ConversationSets.FindIndex(s => s == set);
            if (index < 0)
            {
                details.Add(new Label($"Select a conversation from the dropdown above"));
                return details;
            }
            
            var serializedObject = new SerializedObject(config);
            var setsProp = serializedObject.FindProperty(nameof(config.ConversationSets));
            
            var setProp = setsProp.GetArrayElementAtIndex(index);
            
            var nameField = new PropertyField(setProp.FindPropertyRelative(nameof(set.setName)));
            nameField.Bind(serializedObject);
            details.Add(nameField);
            
            var priorityField = new PropertyField(setProp.FindPropertyRelative(nameof(set.priority)));
            priorityField.Bind(serializedObject);
            details.Add(priorityField);
            
            return details;
        }

        private void PopulateNewSetDetails(VisualElement display, DialogueConfig config)
        {
            display.Clear();

            if (isCreatingNew)
            {
                var setName = new TextField("New set name");
                setName.RegisterValueChangedCallback(e => newSetName = e.newValue);
                display.Add(setName);

                var confirmButton = new Button() {text = "Create"};
                confirmButton.clicked += () =>
                {
                    panel.RecordUndo("Add new conversation set");
                    config.ConversationSets.Add(new DialogueConfig.ConversationSet() { setName = newSetName });
                    isCreatingNew = false;
                    PopulateNewSetDetails(display, config);
                };
                
                var cancelButton = new Button() {text = "Cancel"};
                cancelButton.clicked += () =>
                {
                    isCreatingNew = false;
                    PopulateNewSetDetails(display, config);
                };
                
                display.Add(confirmButton);
                display.Add(cancelButton);
            }
            else
            {
                var createButton = new Button() {text = "Create new set"};
                createButton.clicked += () =>
                {
                    isCreatingNew = true;
                    PopulateNewSetDetails(display, config);
                };
            
                display.Add(createButton);
            }
        }
    }
}