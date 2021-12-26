using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aspekt.Editors;
using HollowForest.AI.States;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class ModulePage : Page<AIEditor, AIEditorData>
    {
        public override string Title => "Modules";

        private NodeEditor nodeEditor;
        private ModuleSidePanel moduleSidePanel;

        private AIModule module;
        private Node selectedNode;

        public AIModule GetSelectedModule() => module;
        
        public ModulePage(AIEditor editor) : base(editor)
        {
        }

        public override void UpdateContents()
        {
            moduleSidePanel.Populate(Editor.Modules);
            nodeEditor.UpdateContents();
            
            if (module == null) return;
            
            var actionNodes = new List<ActionNode>();
            ActionNode defaultAction = null;
            var actionTransitions = new Dictionary<Node, List<Node>>();
            var interruptTransitions = new Dictionary<Node, List<Node>>();
            
            var entryNode = new EntryNode(Editor.Data.GetEntryNodeGuid());
            var interruptNode = new InterruptNode(this, Editor.Data.GetInterruptNodeGuid());

            foreach (var action in module.actions)
            {
                actionNodes.Add(new ActionNode(this, action));
            }

            foreach (var node in actionNodes)
            {
                foreach (var actionNode in actionNodes)
                {
                    if (actionNode.IsTransitionedFrom(node))
                    {
                        AddTransition(actionTransitions, node, actionNode);
                    }
                }
                
                if (module.defaultActionGuid == node.Action.guid)
                {
                    defaultAction = node;
                }

                foreach (var interrupt in module.interrupts)
                {
                    if (interrupt.actionGuid == node.Action.guid)
                    {
                        AddTransition(interruptTransitions, interruptNode, node);
                    }
                }
            }

            if (defaultAction != null)
            {
                nodeEditor.AddNodeDependency(defaultAction, entryNode, entryNode.GetDependencyProfile(AINodeProfiles.DefaultTransition));
            }
            
            foreach (var transition in interruptTransitions)
            {
                foreach (var node in transition.Value)
                {
                    nodeEditor.AddNodeDependency(transition.Key, node, node.GetDependencyProfile(AINodeProfiles.InterruptTransition));
                }
            }

            foreach (var transition in actionTransitions)
            {
                foreach (var actionNode in transition.Value)
                {
                    nodeEditor.AddNodeDependency(transition.Key, actionNode, actionNode.GetDependencyProfile(AINodeProfiles.ActionTransition));
                }
            }

            nodeEditor.AddNode(entryNode);
            nodeEditor.AddNode(interruptNode);
            
            foreach (var node in actionNodes)
            {
                nodeEditor.AddNode(node);
            }
        }

        private void AddTransition(Dictionary<Node, List<Node>> transitions, Node fromNode, Node toNode)
        {
            if (transitions.ContainsKey(toNode))
            {
                transitions[toNode].Add(fromNode);
            }
            else
            {
                transitions.Add(toNode, new List<Node> { fromNode });
            }
        }

        public void SelectModule(AIModule newModule)
        {
            if (newModule == module) return;
            module = newModule;
            Editor.Data.aiModuleID = module.name;
            UpdateContents();
        }

        public void BeginLinkCreation(Node fromNode, int dependencyTypeID)
        {
            nodeEditor.StartDependencyModification(fromNode, dependencyTypeID, NodeEditor.DependencyMode.Create);
        }

        public void BeginLinkRemoval(Node fromNode, int dependencyTypeID)
        {
            nodeEditor.StartDependencyModification(fromNode, dependencyTypeID, NodeEditor.DependencyMode.Remove);
        }

        public void SetDefaultAction(ActionNode node)
        {
            module.defaultActionGuid = node.Action.guid;
            UpdateContents();
        }

        public bool AddInterrupt(ActionNode node)
        {
            if (module.interrupts.Any(i => i.actionGuid == node.Action.guid)) return false;
            
            RecordModuleUndo("Add interrupt");
            module.interrupts.Add(new AIAction.Transition(node.Action.guid));
            return true;
        }

        public bool RemoveInterrupt(ActionNode node)
        {
            var interruptIndex = module.interrupts.FindIndex(i => i.actionGuid == node.Action.guid);
            if (interruptIndex >= 0)
            {
                RecordModuleUndo("Remove interrupt");
                module.interrupts.RemoveAt(interruptIndex);
                UpdateContents();
                return true;
            }

            return false;
        }

        public void RecordModuleUndo(string undoMessage) => Editor.RecordUndo(module, undoMessage);

        protected override void SetupUI(VisualElement root)
        {
            var nodesStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Nodes.uss");
            var sidePanelStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/SidePanel.uss");
            var actionStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/ActionNode.uss");
            root.styleSheets.Add(nodesStyleSheet);
            root.styleSheets.Add(sidePanelStyleSheet);
            root.styleSheets.Add(actionStyleSheet);

            var page = new VisualElement();
            page.AddToClassList("node-page");
            root.Add(page);

            moduleSidePanel = new ModuleSidePanel(this, page);

            nodeEditor = new NodeEditor();
            nodeEditor.NodeSelected += OnNodeSelected;
            nodeEditor.OnNodeUnselected += OnNodeUnselected;
            nodeEditor.SetNodeList(Editor.Data.nodes);
            page.Add(nodeEditor.Element);
            
            var mi = typeof(ModulePage).GetMethod(nameof(CreateNewAction), BindingFlags.NonPublic | BindingFlags.Instance);
            var actionTypes = typeof(AIAction).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(AIAction)));
            foreach (var actionType in actionTypes)
            {
                var newActionMethod = mi.MakeGenericMethod(actionType);
                nodeEditor.AddContextMenuItem(
                    $"Create Action/{actionType.Name}",
                    pos => newActionMethod.Invoke(this, new[] {pos})
                );
            }
            
            nodeEditor.AddContextMenuItem("Reset Zoom", (pos) => nodeEditor.ResetZoom());
            nodeEditor.AddContextMenuItem("Find Starting Node", pos => nodeEditor.FindNodeZero());
            
            SelectModule(Editor.Modules[0]);
            UpdateContents();
        }
        
        private void CreateNewAction<T>(object mousePos) where T : AIAction
        {
            var newAction = ScriptableObject.CreateInstance<T>();
            newAction.name = typeof(T).Name;
            newAction.guid = Guid.NewGuid().ToString();
            
            Editor.RecordUndo(module, "Add new action");

            if (string.IsNullOrEmpty(module.defaultActionGuid) || module.actions.Count == 0)
            {
                module.defaultActionGuid = newAction.guid;
            }

            module.actions.Add(newAction);
            AssetDatabase.AddObjectToAsset(newAction, module);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(module));
            
            var node = new ActionNode(this, newAction);
            nodeEditor.AddNode(node);
            node.SetPosition((Vector2)mousePos);
        }

        public void RemoveAction(ActionNode node)
        {
            Editor.RecordUndo(module, "Remove action");

            module.actions.Remove(node.Action);
            if (module.defaultActionGuid == node.Action.guid)
            {
                module.defaultActionGuid = module.actions.Any() ? module.actions[0].guid : "";
            }
            
            AssetDatabase.RemoveObjectFromAsset(node.Action);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(module));
            nodeEditor.RemoveNode(node);
        }

        private void OnNodeSelected(Node node)
        {
            selectedNode = node;
            moduleSidePanel.OnNodeSelected(node);
        }

        private void OnNodeUnselected(Node node)
        {
            if (selectedNode == node)
            {
                selectedNode = null;
                moduleSidePanel.OnNodeUnselected(node);
            }
        }
    }
}