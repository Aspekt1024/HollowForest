using Aspekt.Editors;
using HollowForest.Data;
using HollowForest.Dialogue.Pages;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    public class DialogueEditor : TabbedEditor<DialogueEditor, DialogueEditorData>
    {
        public string DirectoryRoot => "Assets/Scripts/Editor/Dialogue";
        public override string WindowName => "Dialogue";
        
        public Configuration Config { get; set; }

        [MenuItem("Tools/Dialogue Editor _%#L")]
        private static void ShowWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.Show();
            window.minSize = new Vector2(450f, 300f);
        }

        protected override void OnPreEnable()
        {
            if (!string.IsNullOrEmpty(Data.configPath))
            {
                Config = AssetDatabase.LoadAssetAtPath<Configuration>(Data.configPath);
            }
        }

        protected override void OnPostEnable()
        {
            this.SetAntiAliasing(4);
        }

        protected override void AddPages(VisualElement root)
        {
            AddPage(new EventsPage(this, root));
            AddPage(new NodePage(this, root));
            AddPage(new ConfigPage(this, root));
        }


    }
}