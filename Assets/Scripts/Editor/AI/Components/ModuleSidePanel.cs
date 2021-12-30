using System.Collections.Generic;
using System.Linq;
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

            nodeAttributes = new VisualElement();
            topSection.Add(nodeAttributes);
            
            nodeDetails = new VisualElement();
            nodeDetails.AddToClassList("inspector");
            topSection.Add(nodeDetails);

            
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
            nodeAttributes.ClearClassList();
            selectedNode = node;
            
            if (node != null)
            {
                var isPopulated = node.PopulateAttributeEditor(nodeAttributes, false);
                if (isPopulated)
                {
                    nodeAttributes.AddToClassList("inspector");
                }
                node.PopulateInspector(nodeDetails);
            }
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

            var dropdown = new PopupField<AIModule>(modules, page.Module,
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