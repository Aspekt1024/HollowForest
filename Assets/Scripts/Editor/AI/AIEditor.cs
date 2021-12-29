using System.Collections.Generic;
using Aspekt.Editors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace HollowForest.AI
{
    public class AIEditor : TabbedEditor<AIEditor, AIEditorData>
    {
        public string DirectoryRoot => "Assets/Scripts/Editor/AI";

        public List<AIModule> Modules => Data.ModuleCache;
        
        [MenuItem("Tools/AI Editor _%#K")]
        private static void ShowWindow()
        {
            var window = GetWindow<AIEditor>();
            window.titleContent = new GUIContent("AI");
            window.minSize = new Vector2(450f, 300f);
            window.Show();
        }

        protected override void OnPostEnable()
        {
            this.SetAntiAliasing(4);
        }
        
        protected override void AddPages()
        {
            AddPage(new ModuleEditorPage(this));
            AddPage(new DiagnosticsPage(this));
            AddPage(new LogPage(this));
        }
    }
}