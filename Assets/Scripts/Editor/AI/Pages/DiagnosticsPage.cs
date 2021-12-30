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

        private DiagnosticsSidePanel sidePanel;

        public AIAgent Agent => Editor.Data.selectedAgent;
        
        public DiagnosticsPage(AIEditor editor) : base(editor)
        {
        }

        public override bool CanEdit => false;

        protected override void OnUpdateContents()
        {
            if (Application.isPlaying)
            {
                SelectAgent(Editor.Data.selectedAgent);
                
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
            
            sidePanel = new DiagnosticsSidePanel(this, page, SelectAgent);
        }


        protected override void PostNodeEditorUISetup()
        {
            UpdateContents();
        }

        private void SelectAgent(AIAgent agent)
        {
            var success = Editor.Data.SetAgent(agent);
            if (success)
            {
                sidePanel.SetAgent(agent);
                SelectModule(agent.GetRunningModule());
            }
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