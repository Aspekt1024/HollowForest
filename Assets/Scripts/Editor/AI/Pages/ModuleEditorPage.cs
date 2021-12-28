using Aspekt.Editors;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class ModuleEditorPage : ModuleBasePage
    {
        public override string Title => "Modules";

        private ModuleSidePanel moduleSidePanel;

        public override bool CanEdit => true;
        
        public ModuleEditorPage(AIEditor editor) : base(editor)
        {
        }

        protected override void PreNodeEditorUISetup(VisualElement page)
        {
            moduleSidePanel = new ModuleSidePanel(this, page);
        }

        protected override void PostNodeEditorUISetup()
        {
            SelectModule(Module == null ? Editor.Modules[0] : Module);
        }

        protected override void OnUpdateContents()
        {
            moduleSidePanel.Populate(Editor.Modules);
        }

        public override void OnClear()
        {
            Editor.Data.OnModulePageCleared();
        }

        protected override void OnNodeSelected(Node node)
        {
            moduleSidePanel.OnNodeSelected(node);
        }

        protected override void OnNodeUnselected(Node node)
        {
            moduleSidePanel.OnNodeUnselected(node);
        }
    }
}