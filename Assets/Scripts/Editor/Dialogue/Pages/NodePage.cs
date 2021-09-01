using System;
using System.Linq;
using Aspekt.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class NodePage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Nodes";

        private NodeEditor nodeEditor;

        private int setIndex = 0;
        
        public NodePage(DialogueEditor editor, VisualElement root) : base(editor, root)
        {
        }

        public override void UpdateContents()
        {
            nodeEditor.Clear();
            
            if (Editor.Config == null || Editor.Config.dialogue == null) return;
            var config = Editor.Config.dialogue;

            setIndex = Mathf.Clamp(setIndex, 0, config.ConversationSets.Count - 1);
            if (!config.ConversationSets.Any()) return;

            foreach (var conversation in config.ConversationSets[setIndex].conversations)
            {
                if (string.IsNullOrEmpty(conversation.dialogueGuid))
                {
                    conversation.dialogueGuid = Guid.NewGuid().ToString();
                }
                var node = new DialogueNode(this, conversation);
                nodeEditor.AddNode(node);
            }
        }

        public void RemoveConversation(DialogueConfig.Conversation conversation)
        {
            var index = Editor.Config.dialogue.ConversationSets[setIndex].conversations
                .FindIndex(c => c.dialogueGuid == conversation.dialogueGuid);

            if (index >= 0)
            {
                Editor.RecordUndo(Editor.Config, "Remove Dialogue");
                Editor.Config.dialogue.ConversationSets[setIndex].conversations.RemoveAt(index);
            }
            
            nodeEditor.RemoveNode(conversation.dialogueGuid);
        }

        protected override void SetupUI(VisualElement root)
        {
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Nodes.uss");
            root.styleSheets.Add(stylesheet);

            nodeEditor = new NodeEditor(600, 500);
            nodeEditor.SetNodeList(Editor.Data.nodes);
            root.Add(nodeEditor.GetElement());
            
            nodeEditor.AddContextMenuItem("Create Dialogue", CreateNewDialogue);
            
            UpdateContents();
        }

        private void CreateNewDialogue(object mousePos)
        {
            var newConversation = new DialogueConfig.Conversation();
            Editor.RecordUndo(Editor.Config, "Add new dialogue");
            
            Editor.Config.dialogue.ConversationSets[setIndex].conversations.Add(newConversation);
            
            var node = new DialogueNode(this, newConversation);
            nodeEditor.AddNode(node);
            node.SetPosition((Vector2)mousePos);
        }
    }
}