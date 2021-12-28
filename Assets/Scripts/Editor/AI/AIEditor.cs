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
        public string ModulesDirectory => "Assets/Config/Characters/AI";
        
        public List<AIModule> Modules { get; set; }
        
        [MenuItem("Tools/AI Editor _%#K")]
        private static void ShowWindow()
        {
            var window = GetWindow<AIEditor>();
            window.titleContent = new GUIContent("AI");
            window.minSize = new Vector2(450f, 300f);
            window.Show();
        }

        protected override void OnPreEnable()
        {
            if (!string.IsNullOrEmpty(ModulesDirectory))
            {
                Modules = new List<AIModule>();

                var moduleType = typeof(AIModule).FullName;
                var moduleGUIDs = AssetDatabase.FindAssets($"t:{moduleType}", new [] { ModulesDirectory });
                foreach (var moduleGUID in moduleGUIDs)
                {
                    var path = AssetDatabase.GUIDToAssetPath(moduleGUID);
                    var module = AssetDatabase.LoadAssetAtPath<AIModule>(path);
                    Modules.Add(module);
                }
            }
        }

        protected override void OnPostEnable()
        {
            this.SetAntiAliasing(4);
        }
        
        protected override void AddPages()
        {
            AddPage(new ModulePage(this));
            AddPage(new DiagnosticsPage(this));
        }
    }
}