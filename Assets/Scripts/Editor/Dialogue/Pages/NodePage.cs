using System;
using System.Collections.Generic;
using Aspekt.Editors;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class NodePage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Nodes"; 
        
        public NodePage(DialogueEditor editor, VisualElement root) : base(editor, root)
        {
        }

        public override void UpdateContents()
        {
        }

        protected override void SetupUI(VisualElement root)
        {
            var nodeEditor = new NodeEditor(600, 500);
            nodeEditor.SetNodeList(Editor.Data.nodes);
            root.Add(nodeEditor.GetElement());

            var node = new DialogueNode(new Guid("00000000-0000-0000-0000-000000000000"));
            nodeEditor.AddNode(node);
        }
    }
}