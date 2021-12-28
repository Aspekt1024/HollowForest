using System.Collections.Generic;
using Aspekt.Editors;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class DiagnosticsSidePanel
    {
        private readonly DiagnosticsPage page;
        private readonly VisualElement panel;

        private readonly VisualElement topSection;
        private readonly VisualElement bottomSection;

        private readonly VisualElement agentList;
        private readonly VisualElement nodeDetails;

        private Node selectedNode;
        
        public DiagnosticsSidePanel(DiagnosticsPage page, VisualElement rootElement)
        {
            this.page = page;
            
            panel = new VisualElement();
            panel.AddToClassList("side-panel");

            topSection = new VisualElement();
            bottomSection = new VisualElement();
            panel.Add(topSection);
            panel.Add(bottomSection);

            agentList = new VisualElement();
            topSection.Add(agentList);

            nodeDetails = new VisualElement();
            nodeDetails.AddToClassList("inspector");
            topSection.Add(nodeDetails);
            
            rootElement.Add(panel);
        }
        
        public void Populate(List<AIAgent> agents)
        {
            CreateAIAgentSelection(agents);
            OnNodeSelected(selectedNode);
        }

        public void PopulateOffline()
        {
            agentList.Clear();
            nodeDetails.Clear();
            nodeDetails.Add(new Label("only available during runtime"));
        }

        public void OnNodeSelected(Node node)
        {
            nodeDetails.Clear();
            selectedNode = node;
            node?.PopulateInspector(nodeDetails);
        }

        public void OnNodeUnselected(Node node)
        {
            nodeDetails.Clear();
            selectedNode = null;
        }

        private void CreateAIAgentSelection(List<AIAgent> agents)
        {
            var currentAgentIndex = Mathf.Max(agents.FindIndex(a => a == page.Agent), 0);
            var agentIndexes = new List<int>();
            for (int i = 0; i < agents.Count; i++)
            {
                agentIndexes.Add(i);
            }
            var dropdown = new PopupField<int>(agentIndexes, currentAgentIndex,
            agentIndex =>
            {
                page.SelectAgent(agents[agentIndex]);
                return agents[agentIndex].character.name;
            },
            agentIndex => agents[agentIndex].character.name);

            agentList.Clear();
            agentList.Add(dropdown);
        }
    }
}