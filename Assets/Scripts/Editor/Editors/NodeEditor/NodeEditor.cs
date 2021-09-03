using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public class NodeEditor : MouseManipulator
    {
        public const float GridStepSize = 10f;
        
        private const float MinZoom = 0.2f;
        private const float MaxZoom = 2f;
        
        public float Zoom
        {
            get => zoom;
            private set
            {
                zoom = Mathf.Clamp(value, MinZoom, MaxZoom);
                graph.transform.scale = Vector3.one * zoom;
            }
        }

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

        private float zoom;
        private Vector3 offset;
        private bool isDragging;
        public Action<Vector2> OnDrag;
        
        private readonly VisualElement element;
        private readonly VisualElement graph;
        private readonly ContextMenu contextMenu;

        public VisualElement Element => element;
        
        public NodeEditor()
        {
            contextMenu = new ContextMenu();
            
            element = new VisualElement();
            element.AddManipulator(this);

            graph = new VisualElement();
            graph.AddToClassList("graph");
            element.Add(graph);

            Zoom = 1;
        }

        public void SetNodeList(List<Node> nodes)
        {
            this.nodes = nodes;
        }
        
        public void UpdateContents()
        {
            graph.Clear();
            element.AddToClassList("node-editor");

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/Editors/NodeEditor/NodeEditor.uss");
            element.styleSheets.Add(styleSheet);
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
            
            graph.Add(node.GetElement());
        }

        public void RemoveNode(string guid)
        {
            var index = nodes.FindIndex(n => n.serializableGuid == guid);
            if (index >= 0)
            {
                if (nodes[index].HasElement && graph.Contains(nodes[index].GetElement()))
                {
                    graph.Remove(nodes[index].GetElement());
                    for (int i = dependencyLinks.Count - 1; i >= 0; i--)
                    {
                        if (dependencyLinks[i].IsConnectedToNode(nodes[index]))
                        {
                            graph.Remove(dependencyLinks[i]);
                            dependencyLinks[i].Input.RemoveDependency(dependencyLinks[i].Output);
                            RemoveNodeDependency(dependencyLinks[i].Input, dependencyLinks[i].Output, nodes[index].GetDependencyProfile(dependencyLinks[i].DependencyTypeID));
                        }
                    }
                }
                nodes.RemoveAt(index);
            }
        }
        
        public void AddContextMenuItem(string label, GenericMenu.MenuFunction2 function) => contextMenu.AddContextMenuItem(label, function);

        public void ResetZoom() => Zoom = 1;

        public void FindNodeZero()
        {
            var elementSize = element.layout.size;
            var nodeSize = nodes[0].GetSize();
            offset = elementSize * 0.5f - (nodes[0].GetPosition() + nodeSize * 0.5f) * Zoom;
            graph.transform.position = offset;
        }
        
        private void OnDependencyDrag(Vector2 mousePos)
        {
            var pos = (mousePos + new Vector2(offset.x, offset.y)) / Zoom;
            OnDrag?.Invoke(pos);
        }

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
            target.RegisterCallback<WheelEvent>(OnMouseWheel);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<WheelEvent>(OnMouseWheel);
        }

        private void OnMouseWheel(WheelEvent e)
        {
            var delta = e.delta.y * -0.025f;
            var prevZoom = Zoom;
            
            Zoom += delta;
            if (prevZoom > MinZoom && prevZoom < MaxZoom)
            {
                var mouseOffset = (element.layout.size / 2 - e.localMousePosition) * 0.1f;
                offset += new Vector3(mouseOffset.x, mouseOffset.y, 0f);
                graph.transform.position = offset;
                e.StopPropagation();
            }
        }
        
        private void OnMouseDown(MouseDownEvent e)
        {
            if (e.button == 0)
            {
                isDragging = true;
                element.CaptureMouse();
                e.StopPropagation();
            }
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (e.button == 0)
            {
                isDragging = false;
                element.ReleaseMouse();
                e.StopPropagation();
            }
            if (e.button == 1)
            {
                if (dependencyNode != null)
                {
                    EndDependencyActions();
                    e.StopPropagation();
                    return;
                }
                
                contextMenu.ShowContextMenu((e.localMousePosition - new Vector2(offset.x, offset.y)) / Zoom);
                e.StopPropagation();
            }
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (isDragging)
            {
                offset += new Vector3(e.mouseDelta.x, e.mouseDelta.y, 0f);
                graph.transform.position = offset;
            }
            else
            {
                OnDependencyDrag(e.localMousePosition);
            }
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
            graph.Add(line);
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
            
            if (graph.Contains(dependencyLinks[index]))
            {
                graph.Remove(dependencyLinks[index]);
            }
            dependencyLinks.RemoveAt(index);
        }

        public void StartDependencyModification(Node newDependencyNode, int dependencyTypeID, DependencyMode mode)
        {
            EndDependencyActions();
            
            dependencyMode = mode;
            this.dependencyTypeID = dependencyTypeID;
            
            dependencyNode = newDependencyNode;
            newDependencyNode.ActivatingLinkStart();
            
            var dependencyProfile = newDependencyNode.GetDependencyProfile(dependencyTypeID);
            depLine = new ConnectionElement(this, newDependencyNode, dependencyProfile);
            
            graph.Add(depLine);
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
                if (graph.Contains(depLine)) graph.Remove(depLine);
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