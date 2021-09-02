using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public class NodeEditor : MouseManipulator
    {
        private readonly VisualElement element;
        private readonly ContextMenu contextMenu;

        public Vector2 Size { get; private set; }

        private List<Node> nodes;
        
        public enum DependencyMode
        {
            Create,
            Remove
        }

        private DependencyMode dependencyMode;
        private int dependencyTypeID;
        
        private readonly List<ConnectionElement> dependencyLinks = new List<ConnectionElement>();
        private Node selectedNode;
        private Node dependencyNode;
        private Node lastNode;
        private ConnectionElement depLine;
        
        public Action<Vector2> OnDrag;

        public VisualElement Element => element;
        
        public NodeEditor(int width, int height)
        {
            contextMenu = new ContextMenu();
            Size = new Vector2(width, height);
            
            element = new VisualElement();
            element.AddManipulator(this);
        }

        public void SetNodeList(List<Node> nodes)
        {
            this.nodes = nodes;
        }
        
        public void UpdateContents()
        {
            element.Clear();
            element.AddToClassList("node-editor");

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/Editors/NodeEditor/NodeEditor.uss");
            element.styleSheets.Add(styleSheet);
            
            element.style.width = Size.x;
            element.style.height = Size.y;
        }

        public void AddNode(Node node)
        {
            node.Init(this);
            
            var serializedNode = GetNode(new Guid(node.serializableGuid));
            if (serializedNode != null)
            {
                node.SetBaseData(serializedNode);
                RemoveNode(serializedNode.serializableGuid);
            }
            
            nodes.Add(node);
            node.OnMove += OnNodeMoved;
            node.OnEnter += NodeEntered;
            node.OnLeave += NodeLeft;
            node.OnClick += NodeClicked;
            
            element.Add(node.GetElement());
        }

        public void RemoveNode(string guid)
        {
            var index = nodes.FindIndex(n => n.serializableGuid == guid);
            if (index >= 0)
            {
                if (nodes[index].HasElement && element.Contains(nodes[index].GetElement()))
                {
                    element.Remove(nodes[index].GetElement());
                    for (int i = dependencyLinks.Count - 1; i >= 0; i--)
                    {
                        if (dependencyLinks[i].IsConnectedToNode(nodes[index]))
                        {
                            element.Remove(dependencyLinks[i]);
                            dependencyLinks[i].Input.RemoveDependency(dependencyLinks[i].Output);
                            RemoveNodeDependency(dependencyLinks[i].Input, dependencyLinks[i].Output, nodes[index].GetDependencyProfile(dependencyLinks[i].DependencyTypeID));
                        }
                    }
                }
                nodes.RemoveAt(index);
            }
        }
        
        public void AddContextMenuItem(string label, GenericMenu.MenuFunction2 function) => contextMenu.AddContextMenuItem(label, function);

        private void OnDependencyDrag(Vector2 mousePos) => OnDrag?.Invoke(mousePos);
        
        private Node GetNode(Guid guid)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(nodes[i].serializableGuid))
                {
                    nodes.RemoveAt(i);
                    continue;
                }
                
                if (new Guid(nodes[i].serializableGuid) == guid)
                {
                    return nodes[i];
                }
            }
            
            return null;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        }
        
        private void OnMouseDown(MouseDownEvent e)
        {
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (e.button == 1)
            {
                if (dependencyNode != null)
                {
                    EndDependencyActions();
                    e.StopPropagation();
                    return;
                }
                
                contextMenu.ShowContextMenu(e.mousePosition); // TODO account for nodeEditor position, which is not at 0,0
                e.StopPropagation();
            }
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            OnDependencyDrag(e.localMousePosition);
        }

        private void OnNodeMoved(Node node)
        {
        }

        /// <summary>
        /// Adds a visual representation of a dependency between two nodes.
        /// This assumes the dependency has already been (or will be) established in data.
        /// </summary>
        public void AddNodeDependency(Node node, Node dependency, Node.DependencyProfile dependencyProfile)
        {
            var line = new ConnectionElement(dependency, node, dependencyProfile);
            dependencyLinks.Add(line);
            element.Add(line);
        }

        /// <summary>
        /// Removes a visual representation of a dependency between two nodes.
        /// This assumes the dependency has already been (or will be) removed in data.
        /// </summary>
        private void RemoveNodeDependency(Node node, Node dependency, Node.DependencyProfile dependencyProfile)
        {
            var index = dependencyLinks.FindIndex(l => 
                l.DependencyTypeID == dependencyProfile.dependencyTypeID && l.Output == dependency && l.Input == node);
            
            if (index < 0) return;
            
            if (element.Contains(dependencyLinks[index]))
            {
                element.Remove(dependencyLinks[index]);
            }
            dependencyLinks.RemoveAt(index);
        }

        public void StartDependencyModification(Node newDependencyNode, Vector2 mousePos, int dependencyTypeID, DependencyMode mode)
        {
            EndDependencyActions();
            
            dependencyMode = mode;
            this.dependencyTypeID = dependencyTypeID;
            
            dependencyNode = newDependencyNode;
            newDependencyNode.ActivatingLinkStart();
            
            var dependencyProfile = newDependencyNode.GetDependencyProfile(dependencyTypeID);
            depLine = new ConnectionElement(this, newDependencyNode, mousePos, dependencyProfile);
            
            element.Add(depLine);
        }

        private void EndDependencyModification(Node dependentNode)
        {
            if (dependencyNode != null)
            {
                if (dependentNode != null)
                {
                    var success = false;
                    if (dependencyMode == DependencyMode.Create)
                    {
                        success = dependentNode.CreateDependency(dependencyNode);
                        if (success)
                        {
                            AddNodeDependency(dependentNode, dependencyNode, dependencyNode.GetDependencyProfile(dependencyTypeID));
                        }
                    }
                    else if (dependencyMode == DependencyMode.Remove)
                    {
                        success = dependentNode.RemoveDependency(dependencyNode);
                        if (success)
                        {
                            RemoveNodeDependency(dependentNode, dependencyNode, dependencyNode.GetDependencyProfile(dependencyTypeID));
                        }
                    }
                    
                    dependentNode.ActivatingLinkEnd();
                }
            }
            
            EndDependencyActions();
        }

        private void EndDependencyActions()
        {
            if (dependencyNode != null)
            {
                dependencyNode.ActivatingLinkEnd();
                dependencyNode = null;
            }

            lastNode?.ActivatingLinkEnd();

            if (depLine != null)
            {
                if (element.Contains(depLine)) element.Remove(depLine);
                depLine = null;
            }
        }
        
        private void NodeEntered(Node node)
        {
            if (dependencyNode != null)
            {
                node.ActivatingLinkStart();
            }
            
            lastNode = node;
        }

        private void NodeLeft(Node node)
        {
            if (dependencyNode != null && dependencyNode != node)
            {
                node.ActivatingLinkEnd();
            }

            if (lastNode == node)
            {
                lastNode = null;
            }
        }

        private void NodeClicked(Node node)
        {
            selectedNode?.ShowUnselected();
            node.ShowSelected();
            selectedNode = node;

            if (dependencyNode != null && dependencyNode != node)
            {
                EndDependencyModification(node);
            }
        }
        
    }
}