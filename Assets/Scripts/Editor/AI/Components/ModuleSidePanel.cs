using System.Collections.Generic;
using Aspekt.Editors;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class ModuleSidePanel
    {
        private readonly ModuleEditorPage page;
        private readonly VisualElement panel;

        private readonly VisualElement topSection;
        private readonly VisualElement bottomSection;

        private readonly VisualElement moduleList;
        private readonly VisualElement nodeDetails;
        private readonly VisualElement nodeAttributes;

        private Node selectedNode;
        
        public ModuleSidePanel(ModuleEditorPage page, VisualElement rootElement)
        {
            this.page = page;
            
            panel = new VisualElement();
            panel.AddToClassList("side-panel");

            topSection = new VisualElement();
            bottomSection = new VisualElement();
            panel.Add(topSection);
            panel.Add(bottomSection);

            moduleList = new VisualElement();
            topSection.Add(moduleList);

            nodeDetails = new VisualElement();
            nodeDetails.AddToClassList("inspector");
            topSection.Add(nodeDetails);

            nodeAttributes = new VisualElement();
            nodeAttributes.AddToClassList("inspector");
            bottomSection.Add(nodeAttributes);
            
            rootElement.Add(panel);
        }
        
        public void Populate(List<AIModule> modules)
        {
            CreateModuleSelection(modules);
            OnNodeSelected(selectedNode);
        }

        public void OnNodeSelected(Node node)
        {
            nodeDetails.Clear();
            nodeAttributes.Clear();
            selectedNode = node;
            node?.PopulateInspector(nodeDetails);
            node?.PopulateAttributeEditor(nodeAttributes);
        }

        public void OnNodeUnselected(Node node)
        {
            nodeDetails.Clear();
            nodeAttributes.Clear();
            selectedNode = null;
        }

        private void CreateModuleSelection(List<AIModule> modules)
        {
            moduleList.Clear();

            var currentModule = page.Module;
            var dropdown = new PopupField<AIModule>(modules, currentModule,
            m =>
            {
                page.SelectModule(m);
                return m.name;
            },
            m => m.name);
            moduleList.Add(dropdown);
        }
    }
}