using Aspekt.Editors;
using HollowForest.Data;
using HollowForest.Dialogue.Pages;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace HollowForest.Dialogue
{
    public class DialogueEditor : TabbedEditor<DialogueEditor, DialogueEditorData>
    {
        public string DirectoryRoot => "Assets/Scripts/Editor/Dialogue";
        
        public Configuration Config { get; set; }

        [MenuItem("Tools/Dialogue Editor _%#L")]
        private static void ShowWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Dialogue");
            window.minSize = new Vector2(450f, 300f);
            window.Show();
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

        protected override void AddPages()
        {
            AddPage(new DialoguePage(this));
            AddPage(new EventsPage(this));
            AddPage(new CharacterPage(this));
            AddPage(new ZoneAreaPage(this));
            AddPage(new ItemPage(this));
            AddPage(new ConfigPage(this));
        }
    }
}