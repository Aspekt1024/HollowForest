using System.Collections.Generic;
using Aspekt.Editors;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class ModuleSidePanel
    {
        private readonly ModulePage page;
        private readonly VisualElement panel;

        private readonly VisualElement moduleList;
        
        public ModuleSidePanel(ModulePage page, VisualElement rootElement)
        {
            this.page = page;

            var setInfoElement = new VisualElement();
            var conversationInfoElement = new VisualElement();
            
            panel = new VisualElement();
            
            panel.AddToClassList("side-panel");
            
            panel.Add(setInfoElement);
            panel.Add(conversationInfoElement);

            moduleList = new VisualElement();
            panel.Add(moduleList);
            
            rootElement.Add(panel);
        }
        
        public void Populate(List<AIModule> modules)
        {
            moduleList.Clear();

            foreach (var module in modules)
            {
                moduleList.Add(new Label(module.name));
            }
        }

        public void OnNodeSelected(Node node)
        {
            
        }

        public void OnNodeUnselected(Node node)
        {
            
        }
    }
}