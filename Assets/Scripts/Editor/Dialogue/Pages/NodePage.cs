using System;
using System.Linq;
using Aspekt.Editors;
using UnityEditor;
using UnityEditor.UIElements;
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
            if (Editor.Config == null || Editor.Config.dialogue == null) return;
            var config = Editor.Config.dialogue;

            setIndex = Mathf.Clamp(setIndex, 0, config.ConversationSets.Count - 1);
            if (!config.ConversationSets.Any()) return;

            foreach (var conversation in config.ConversationSets[setIndex].conversations)
            {
                if (string.IsNullOrEmpty(conversation.conversationGuid))
                {
                    conversation.conversationGuid = Guid.NewGuid().ToString();
                }
                var node = new DialogueNode(conversation);
                nodeEditor.AddNode(node);
            }
        }

        protected override void SetupUI(VisualElement root)
        {
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Nodes.uss");
            root.styleSheets.Add(stylesheet);

            nodeEditor = new NodeEditor(600, 500);
            nodeEditor.SetNodeList(Editor.Data.nodes);
            root.Add(nodeEditor.GetElement());
            
            UpdateContents();
        }
    }
}