using Aspekt.Editors;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class TestPage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Test";
        
        public TestPage(DialogueEditor editor, VisualElement root) : base(editor, root)
        {
        }
        
        public override void UpdateContents()
        {
            
        }

        protected override void SetupUI(VisualElement root)
        {
            root.Add(new Label("test page !"));
        }

    }
}