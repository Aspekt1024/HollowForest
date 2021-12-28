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

        private readonly VisualElement agentDetails;

        private Node selectedNode;
        private AIAgent selectedAgent;
        
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

            agentDetails = new VisualElement();
            agentDetails.AddToClassList("inspector");
            bottomSection.Add(agentDetails);
            
            rootElement.Add(panel);
        }
        
        public void Populate(List<AIAgent> agents)
        {
            CreateAIAgentSelection(agents);
            OnNodeSelected(selectedNode);
            PopulateAgentStateDisplay();
        }

        public void PopulateOffline()
        {
            agentList.Clear();
            nodeDetails.Clear();
            nodeDetails.Add(new Label("only available during runtime"));
        }

        public void SetAgent(AIAgent agent)
        {
            if (selectedAgent != null)
            {
                UnregisterAgentObservations(agent);
            }
            selectedAgent = agent;
            RegisterAgentObservations(agent);
            PopulateAgentStateDisplay();
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

        private void RegisterAgentObservations(AIAgent agent)
        {
            agent.memory.RegisterAllStateObserver(OnAIStateChanged);
            agent.memory.RegisterAllObjectObserver(OnAIObjectChanged);
            agent.character.State.RegisterAllStateObserver(OnCharacterStateChanged);
        }

        private void UnregisterAgentObservations(AIAgent agent)
        {
            agent.memory.UnregisterAllStateObserver(OnAIStateChanged);
            agent.memory.UnregisterAllObjectObserver(OnAIObjectChanged);
            agent.character.State.UnregisterAllStateObserver(OnCharacterStateChanged);
        }

        private void OnAIStateChanged(AIState state, bool value) => PopulateAgentStateDisplay();
        private void OnAIObjectChanged(AIObject state, object value) => PopulateAgentStateDisplay();
        private void OnCharacterStateChanged(CharacterStates state, bool value) => PopulateAgentStateDisplay();

        private void PopulateAgentStateDisplay()
        {
            agentDetails.Clear();

            if (selectedAgent == null) return;

            var aiStates = selectedAgent.memory.GetStateCopy();
            var aiObjects = selectedAgent.memory.GetObjectsCopy();
            var charStates = selectedAgent.character.State.GetStateCopy();
            
            foreach (var state in aiStates)
            {
                agentDetails.Add(new Label($"{state.Key}: {state.Value}"));
            }

            foreach (var aiObject in aiObjects)
            {
                agentDetails.Add(new Label($"{aiObject.Key}: {aiObject.Value}"));
            }

            foreach (var state in charStates)
            {
                agentDetails.Add(new Label($"{state.Key}: {state.Value}"));
            }
        }
    }
}