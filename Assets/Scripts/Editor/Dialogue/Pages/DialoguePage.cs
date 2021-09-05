using System;
using System.Collections.Generic;
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
        private DialogueSidePanel dialogueSidePanel;

        private DialogueConfig.ConversationSet conversationSet;
        private Node selectedNode;
        
        public DialoguePage(DialogueEditor editor) : base(editor)
        {
        }

        public override void UpdateContents()
        {
            dialogueSidePanel.Populate(Editor.Config.dialogue, conversationSet);

            nodeEditor.UpdateContents();
            
            if (Editor.Config == null || Editor.Config.dialogue == null) return;
            if (conversationSet == null) return;

            var conversations = conversationSet.conversations;
            
            var dialogueNodes = new List<DialogueNode>();
            var dialogueDependencies = new Dictionary<Node, List<Node>>();
            
            for (int i = 0; i < conversations.Count; i++)
            {
                if (string.IsNullOrEmpty(conversations[i].dialogueGuid))
                {
                    conversations[i].dialogueGuid = Guid.NewGuid().ToString();
                }
                var node = new DialogueNode(this, conversations[i], i);
                dialogueNodes.Add(node);
            }

            foreach (var node in dialogueNodes)
            {
                var dependencies = dialogueNodes.Where(node.IsLinkedTo).Select(n => n as Node).ToList();
                if (dependencies.Any())
                {
                    dialogueDependencies.Add(node, dependencies);
                }
            }

            foreach (var dependency in dialogueDependencies)
            {
                foreach (var dependencyNode in dependency.Value)
                {
                    nodeEditor.AddNodeDependency(dependency.Key, dependencyNode, dependencyNode.GetDependencyProfile(DialogueNodeProfiles.LinkDialogueDependency));
                }
            }
            
            foreach (var node in dialogueNodes)
            {
                nodeEditor.AddNode(node);
            }
        }

        public void SelectConversationSet(DialogueConfig.ConversationSet set)
        {
            if (set == conversationSet) return;
            conversationSet = set;
            Editor.Data.conversationSetGuid = set.setGuid;
            UpdateContents();
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

        public void BeginLinkCreation(DialogueNode fromNode, int dependencyTypeID)
        {
            nodeEditor.StartDependencyModification(fromNode, dependencyTypeID, NodeEditor.DependencyMode.Create);
        }

        public void BeginLinkRemoval(DialogueNode fromNode, int dependencyTypeID)
        {
            nodeEditor.StartDependencyModification(fromNode, dependencyTypeID, NodeEditor.DependencyMode.Remove);
        }

        public void RecordDialogueUndo(string undoMessage) => Editor.RecordUndo(Editor.Config.dialogue, undoMessage);

        protected override void SetupUI(VisualElement root)
        {
            var nodesStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Nodes.uss");
            var sidePanelStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/SidePanel.uss");
            var dialogueStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/DialogueNode.uss");
            root.styleSheets.Add(nodesStyleSheet);
            root.styleSheets.Add(sidePanelStyleSheet);
            root.styleSheets.Add(dialogueStyleSheet);

            var page = new VisualElement();
            page.AddToClassList("node-page");
            root.Add(page);

            dialogueSidePanel = new DialogueSidePanel(this, page);
            conversationSet = DetermineInitialConversationSet();

            nodeEditor = new NodeEditor();
            nodeEditor.NodeSelected += OnNodeSelected;
            nodeEditor.OnNodeUnselected += OnNodeUnselected;
            nodeEditor.SetNodeList(Editor.Data.nodes);
            page.Add(nodeEditor.Element);
            
            nodeEditor.AddContextMenuItem("Create Dialogue", CreateNewDialogue);
            nodeEditor.AddContextMenuItem("Reset Zoom", (pos) => nodeEditor.ResetZoom());
            nodeEditor.AddContextMenuItem("Find Starting Node", pos => nodeEditor.FindNodeZero());
            
            UpdateContents();
        }

        private DialogueConfig.ConversationSet DetermineInitialConversationSet()
        {
            var set = Editor.Config.dialogue.ConversationSets.Any() ? Editor.Config.dialogue.ConversationSets[0] : null;
            if (!string.IsNullOrEmpty(Editor.Data.conversationSetGuid))
            {
                var index = Editor.Config.dialogue.ConversationSets.FindIndex(s => s.setGuid == Editor.Data.conversationSetGuid);
                if (index >= 0)
                {
                    set = Editor.Config.dialogue.ConversationSets[index];
                }
            }

            if (set != null)
            {
                Editor.Data.conversationSetGuid = set.setGuid;
            }
            
            return set;
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

        private void OnNodeSelected(Node node)
        {
            selectedNode = node;
            dialogueSidePanel.OnNodeSelected(node);
        }

        private void OnNodeUnselected(Node node)
        {
            if (selectedNode == node)
            {
                selectedNode = null;
                dialogueSidePanel.OnNodeUnselected(node);
            }
        }
    }
}