using System;
using System.Linq;
using Aspekt.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class DialoguePage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Dialogue";

        private NodeEditor nodeEditor;
        private ConversationSetPanel conversationSetPanel;

        private DialogueConfig.ConversationSet conversationSet;
        private DialogueNode selectedNode;
        
        public DialoguePage(DialogueEditor editor, VisualElement root) : base(editor, root)
        {
        }

        public override void UpdateContents()
        {
            conversationSetPanel.Populate(Editor.Config.dialogue, conversationSet);
            
            nodeEditor.Clear();
            
            if (Editor.Config == null || Editor.Config.dialogue == null) return;

            if (conversationSet == null) return;

            var conversations = conversationSet.conversations;
            for (int i = 0; i < conversations.Count; i++)
            {
                if (string.IsNullOrEmpty(conversations[i].dialogueGuid))
                {
                    conversations[i].dialogueGuid = Guid.NewGuid().ToString();
                }
                var node = new DialogueNode(this, conversations[i], i);
                nodeEditor.AddNode(node);
            }
        }

        public void RemoveConversation(DialogueConfig.Conversation conversation)
        {
            var index = conversationSet.conversations
                .FindIndex(c => c.dialogueGuid == conversation.dialogueGuid);

            if (index >= 0)
            {
                Editor.RecordUndo(Editor.Config.dialogue, "Remove Dialogue");
                conversationSet.conversations.RemoveAt(index);
            }
            
            nodeEditor.RemoveNode(conversation.dialogueGuid);
        }

        public void RecordDialogueUndo(string undoMessage) => Editor.RecordUndo(Editor.Config.dialogue, undoMessage);

        protected override void SetupUI(VisualElement root)
        {
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Nodes.uss");
            root.styleSheets.Add(stylesheet);

            var page = new VisualElement();
            page.AddToClassList("node-page");
            root.Add(page);

            conversationSetPanel = new ConversationSetPanel(this, page);
            conversationSet = Editor.Config.dialogue.ConversationSets.Any() ? Editor.Config.dialogue.ConversationSets[0] : null;

            nodeEditor = new NodeEditor(600, 500);
            nodeEditor.SetNodeList(Editor.Data.nodes);
            page.Add(nodeEditor.GetElement());
            
            nodeEditor.AddContextMenuItem("Create Dialogue", CreateNewDialogue);
            
            UpdateContents();
        }

        private void CreateNewDialogue(object mousePos)
        {
            var newConversation = new DialogueConfig.Conversation();
            Editor.RecordUndo(Editor.Config.dialogue, "Add new dialogue");

            conversationSet.conversations.Add(newConversation);
            
            var node = new DialogueNode(this, newConversation, conversationSet.conversations.Count - 1);
            nodeEditor.AddNode(node);
            node.SetPosition((Vector2)mousePos);
        }
    }
}