using System.Linq;
using Aspekt.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class DiagnosticsPage : ModuleBasePage
    {
        public override string Title => "Diagnostics";

        private NodeEditor nodeEditor;
        private DiagnosticsSidePanel sidePanel;

        public AIAgent Agent;
        
        private Node selectedNode;
        
        public DiagnosticsPage(AIEditor editor) : base(editor)
        {
        }

        public override bool CanEdit => false;

        protected override void OnUpdateContents()
        {
            if (Application.isPlaying)
            {
                SelectModule(Editor.Data.selectedModule);
                
                var agents = Object.FindObjectsOfType<Enemy>()
                    .Where(e => e.GetAI() != null)
                    .Select(e => e.GetAI()).ToList();
            
                sidePanel.Populate(agents);
            }
            else
            {
                sidePanel.PopulateOffline();
                SelectModule(null);
            }
        }

        protected override void PreNodeEditorUISetup(VisualElement page)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            
            sidePanel = new DiagnosticsSidePanel(this, page);
        }


        protected override void PostNodeEditorUISetup()
        {
            UpdateContents();
        }

        public void SelectAgent(AIAgent agent)
        {
            if (Agent == agent) return;
            Agent = agent;
            sidePanel.SetAgent(agent);
            SelectModule(agent.GetRunningModule());
        }

        protected override void OnNodeSelected(Node node)
        {
            sidePanel.OnNodeSelected(node);
        }

        protected override void OnNodeUnselected(Node node)
        {
            sidePanel.OnNodeUnselected(node);
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