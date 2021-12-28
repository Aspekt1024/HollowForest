using Aspekt.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class DiagnosticsPage : Page<AIEditor, AIEditorData>
    {
        public override string Title => "Diagnostics";

        private VisualElement contents;
        
        public DiagnosticsPage(AIEditor editor) : base(editor)
        {
        }

        public override void UpdateContents()
        {
            if (contents == null) return;
            contents.Clear();
            if (Application.isPlaying)
            {
                contents.Add(new Label("Playing"));
            }
            else
            {
                contents.Add(new Label("Not playing"));
            }
        }

        protected override void SetupUI(VisualElement root)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            
            contents = new VisualElement();
            root.Add(contents);
            UpdateContents();
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