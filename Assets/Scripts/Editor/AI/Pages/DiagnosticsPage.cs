using System.Linq;
using Aspekt.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class DiagnosticsPage : Page<AIEditor, AIEditorData>
    {
        public override string Title => "Diagnostics";

        private NodeEditor nodeEditor;
        private DiagnosticsSidePanel sidePanel;

        public AIAgent Agent;
        
        private Node selectedNode;
        
        public DiagnosticsPage(AIEditor editor) : base(editor)
        {
        }

        public override void UpdateContents()
        {
            if (Application.isPlaying)
            {
                var agents = Object.FindObjectsOfType<Enemy>()
                    .Where(e => e.GetAI() != null)
                    .Select(e => e.GetAI()).ToList();
            
                sidePanel.Populate(agents);
                nodeEditor.UpdateContents();
            }
            else
            {
                sidePanel.PopulateOffline();
            }
        }

        protected override void SetupUI(VisualElement root)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            
            var nodesStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Nodes.uss");
            var sidePanelStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/SidePanel.uss");
            var actionStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/ActionNode.uss");
            root.styleSheets.Add(nodesStyleSheet);
            root.styleSheets.Add(sidePanelStyleSheet);
            root.styleSheets.Add(actionStyleSheet);
            
            var page = new VisualElement();
            page.AddToClassList("node-page");
            root.Add(page);
            
            sidePanel = new DiagnosticsSidePanel(this, page);

            nodeEditor = new NodeEditor();
            
            nodeEditor.NodeSelected += OnNodeSelected;
            nodeEditor.OnNodeUnselected += OnNodeUnselected;
            page.Add(nodeEditor.Element);
            
            nodeEditor.AddContextMenuItem("Reset Zoom", (pos) => nodeEditor.ResetZoom());
            nodeEditor.AddContextMenuItem("Find Starting Node", pos => nodeEditor.FindNodeZero());
            
            UpdateContents();
        }

        public void SelectAgent(AIAgent agent)
        {
            if (Agent == agent) return;
            Agent = agent;
            UpdateContents();
        }

        private void OnNodeSelected(Node node)
        {
            selectedNode = node;
            sidePanel.OnNodeSelected(node);
        }

        private void OnNodeUnselected(Node node)
        {
            if (selectedNode == node)
            {
                selectedNode = null;
                sidePanel.OnNodeUnselected(node);
            }
        }

        ~DiagnosticsPage()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            UpdateContents();
        }
    }
}