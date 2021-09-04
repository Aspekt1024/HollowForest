using System;
using Aspekt.Editors;
using HollowForest.Dialogue.Pages;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    public class DialogueSidePanel
    {
        private readonly DialoguePage page;
        private readonly VisualElement panel;
        private readonly VisualElement setInfo;
        private readonly VisualElement inspector;

        private Node selectedNode;
        private DialogueConfig.ConversationSet currentSet;

        public event Action<DialogueConfig.ConversationSet> NewConversationSetSelected = delegate {  };
        
        public DialogueSidePanel(DialoguePage page, VisualElement rootElement)
        {
            this.page = page;
            
            panel = new VisualElement();
            setInfo = new VisualElement();
            inspector = new VisualElement();
            
            panel.AddToClassList("side-panel");
            
            panel.Add(setInfo);
            panel.Add(inspector);
            
            rootElement.Add(panel);
        }
        
        public void Populate(DialogueConfig config, DialogueConfig.ConversationSet currentSet)
        {
            this.currentSet = currentSet;
            PopulateSetInfo(config, currentSet);
        }

        public void OnNodeSelected(Node node)
        {
            selectedNode = node;
            PopulateNodeInfo(node);
        }

        public void OnNodeUnselected(Node node)
        {
            if (selectedNode != node) return;
            
            selectedNode = null;
            inspector.Clear();
        }

        private void PopulateSetInfo(DialogueConfig config, DialogueConfig.ConversationSet set)
        {
            setInfo.Clear();
            
            var setDropdown = CreateSetDropdown(config, set);
            setInfo.Add(setDropdown);

            var setDetails = CreateSetDetails(config, set);
            setInfo.Add(setDetails);
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

        private void PopulateNodeInfo(Node node)
        {
            inspector.Clear();
            
            if (node is DialogueNode dialogueNode)
            {
                inspector.Add(ConversationInspector.GetInspector(dialogueNode, currentSet, page.Editor.Config));
            }
        }
    }
}