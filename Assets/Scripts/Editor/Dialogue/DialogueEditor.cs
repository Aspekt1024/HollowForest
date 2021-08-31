using Aspekt.Editors;
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

        [MenuItem("Tools/Dialogue Editor _%#L")]
        private static void ShowWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.Show();
            window.minSize = new Vector2(450f, 300f);
        }
        
        protected override void OnPostEnable()
        {
            this.SetAntiAliasing(4);
        }

        protected override void AddPages(VisualElement root)
        {
            AddPage(new TestPage(this, root));
            AddPage(new NodePage(this, root));
        }


    }
}