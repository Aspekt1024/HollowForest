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
        
        public ModulePage(AIEditor editor) : base(editor)
        {
        }

        public override void UpdateContents()
        {
            moduleSidePanel.Populate(Editor.Modules);
            nodeEditor.UpdateContents();
            
            if (module == null) return;
            
            var actionNodes = new List<ActionNode>();
            var actionTransitions = new Dictionary<Node, List<Node>>();

            foreach (var action in module.actions)
            {
                actionNodes.Add(new ActionNode(this, action));
            }

            foreach (var node in actionNodes)
            {
                var dependencies = actionNodes.Where(node.IsTransitionedFrom).Select(n => n as Node).ToList();
                if (dependencies.Any())
                {
                    actionTransitions.Add(node, dependencies);
                }
            }

            foreach (var transition in actionTransitions)
            {
                foreach (var actionNode in transition.Value)
                {
                    nodeEditor.AddNodeDependency(transition.Key, actionNode, actionNode.GetDependencyProfile(AINodeProfiles.ActionTransition));
                }
            }
            
            foreach (var node in actionNodes)
            {
                nodeEditor.AddNode(node);
            }
        }

        public void SelectModule(AIModule newModule)
        {
            if (newModule == module) return;
            module = newModule;
            Editor.Data.aiModuleID = module.name;
            UpdateContents();
        }

        public void BeginLinkCreation(ActionNode fromNode, int dependencyTypeID)
        {
            nodeEditor.StartDependencyModification(fromNode, dependencyTypeID, NodeEditor.DependencyMode.Create);
        }

        public void BeginLinkRemoval(ActionNode fromNode, int dependencyTypeID)
        {
            nodeEditor.StartDependencyModification(fromNode, dependencyTypeID, NodeEditor.DependencyMode.Remove);
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