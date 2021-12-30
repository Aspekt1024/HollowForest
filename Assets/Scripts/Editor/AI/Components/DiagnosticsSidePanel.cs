using System;
using System.Collections.Generic;
using Aspekt.Editors;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class DiagnosticsSidePanel
    {
        private readonly Page<AIEditor, AIEditorData> page;
        private readonly VisualElement panel;
        private readonly Action<AIAgent> agentSelectedCallback;

        private readonly VisualElement topSection;
        private readonly VisualElement bottomSection;

        private readonly VisualElement agentList;
        private readonly VisualElement nodeDetails;
        private readonly VisualElement nodeAttributes;
        private readonly VisualElement logFilterDetails;

        private readonly VisualElement agentDetails;

        private Node selectedNode;
        private AIAgent selectedAgent;
        
        public DiagnosticsSidePanel(Page<AIEditor, AIEditorData> page, VisualElement rootElement, Action<AIAgent> agentSelectedCallback)
        {
            this.page = page;
            this.agentSelectedCallback = agentSelectedCallback;
            
            panel = new VisualElement();
            panel.AddToClassList("side-panel");

            topSection = new VisualElement();
            bottomSection = new VisualElement();
            panel.Add(topSection);
            panel.Add(bottomSection);

            agentList = new VisualElement();
            topSection.Add(agentList);
            
            nodeAttributes = new VisualElement();
            topSection.Add(nodeAttributes);

            nodeDetails = new VisualElement();
            nodeDetails.AddToClassList("inspector");
            topSection.Add(nodeDetails);

            logFilterDetails = new VisualElement();
            logFilterDetails.AddToClassList("inspector");
            topSection.Add(logFilterDetails);

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

        public void PopulateLogFilterDisplay(LogFilter filter)
        {
            logFilterDetails.Clear();
            filter.Populate(logFilterDetails);
        }

        public void PopulateOffline()
        {
            agentList.Clear();
            nodeAttributes.Clear();
            nodeDetails.Clear();
            nodeDetails.Add(new Label("Agent list only available during runtime"));
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
            nodeAttributes.Clear();
            nodeAttributes.ClearClassList();
            selectedNode = node;

            if (node != null)
            {
                var isPopulated = node.PopulateAttributeEditor(nodeAttributes, true);
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

        private void CreateAIAgentSelection(List<AIAgent> agents)
        {
            var currentAgentIndex = Mathf.Max(agents.FindIndex(a => a == page.Editor.Data.selectedAgent), 0);
            var agentIndexes = new List<int>();
            for (int i = 0; i < agents.Count; i++)
            {
                agentIndexes.Add(i);
            }
            var dropdown = new PopupField<int>(agentIndexes, currentAgentIndex,
            agentIndex =>
            {
                agentSelectedCallback?.Invoke(agents[agentIndex]);
                return agents[agentIndex].character.name;
            },
            agentIndex => agents[agentIndex].character.name);

            agentList.Clear();
            agentList.Add(dropdown);

            if (selectedAgent != null)
            {
                var module = selectedAgent.GetRunningModule();
                var moduleName = module != null ? module.name.Replace("(Clone)", "") : "None";
                agentList.Add(new Label($"AI Module: {moduleName}"));
            }
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