using System.Linq;
using Aspekt.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class LogPage : Page<AIEditor, AIEditorData>
    {
        public override string Title => "Log";
        
        private DiagnosticsSidePanel sidePanel;
        private AILog aiLog; 
        public AIAgent Agent => Editor.Data.selectedAgent;
        
        public LogPage(AIEditor editor) : base(editor)
        {
            aiLog = new AILog(editor.Data.logFilter);
        }
        
        public override void UpdateContents()
        {
            if (Application.isPlaying)
            {
                var agents = Object.FindObjectsOfType<Enemy>()
                    .Where(e => e.GetAI() != null)
                    .Select(e => e.GetAI()).ToList();
            
                sidePanel.Populate(agents);
                aiLog.Populate();
            }
            else
            {
                sidePanel.PopulateOffline();
            }
            
            sidePanel.PopulateLogFilterDisplay(Editor.Data.logFilter);
        }
        
        private void SelectAgent(AIAgent agent)
        {
            var success = Editor.Data.SetAgent(agent);
            if (success)
            {
                sidePanel.SetAgent(agent);
                aiLog.SetAgent(agent);
            }
        }

        protected override void SetupUI(VisualElement root)
        {
            var sidePanelStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/SidePanel.uss");
            var logStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Log.uss");
            root.styleSheets.Add(sidePanelStyleSheet);
            root.styleSheets.Add(logStyleSheet);

            var page = new VisualElement();
            page.AddToClassList("log-page");
            root.Add(page);
            
            sidePanel = new DiagnosticsSidePanel(this, page, SelectAgent);
            aiLog.Init(page);
            Editor.Data.logFilter.OnUpdated += UpdateContents;
            
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            Editor.Data.AllowAgentReload();
            UpdateContents();
        }

        ~LogPage()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            UpdateContents();
        }
    }
}