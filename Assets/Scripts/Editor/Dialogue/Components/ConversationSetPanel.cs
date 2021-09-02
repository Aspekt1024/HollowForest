using System;
using System.Collections.Generic;
using HollowForest.Dialogue.Pages;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    public class ConversationSetPanel
    {
        private readonly DialoguePage page;
        private readonly VisualElement panel;

        public event Action<DialogueConfig.ConversationSet> NewConversationSetSelected = delegate {  };
        
        public ConversationSetPanel(DialoguePage page, VisualElement rootElement)
        {
            this.page = page;
            
            panel = new VisualElement();
            rootElement.Add(panel);
        }
        
        public void Populate(DialogueConfig config, DialogueConfig.ConversationSet currentSet)
        {
            var serializedObject = new SerializedObject(config);
            
            panel.Clear();
            panel.AddToClassList("side-panel");

            var setDropdown = CreateSetDropdown(config, currentSet);
            panel.Add(setDropdown);

            var setDetails = CreateSetDetails(config, currentSet);
            panel.Add(setDetails);
            

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
            
            var characterField = new PropertyField(setProp.FindPropertyRelative(nameof(set.interactedCharacter)));
            characterField.Bind(serializedObject);
            details.Add(characterField);
            
            return details;
        }
    }
}