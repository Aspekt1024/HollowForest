using System.Linq;
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
            var module = Module;
            if (module != null && !Editor.Modules.Contains(module))
            {
                // Occurs in runtime when the current module is set as a clone from the Diagnostics page
                module = Editor.Modules.FirstOrDefault(m => m.moduleGuid == module.moduleGuid);
            }
            if (module == null)
            {
                module = Editor.Modules[0];
            }
            SelectModule(module);
        }

        protected override void OnUpdateContents()
        {
            moduleSidePanel.Populate(Editor.Modules);
        }

        public override void OnClear()
        {
            Editor.Data.AllowAgentReload();
            Editor.Data.AllowModuleReload();
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