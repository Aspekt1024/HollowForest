using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public class NodeEditor : MouseManipulator
    {
        private VisualElement element;
        
        private readonly ContextMenu contextMenu;

        public Vector2 Size { get; private set; }

        private List<Node> nodes;
        
        public NodeEditor(int width, int height)
        {
            contextMenu = new ContextMenu();
            Size = new Vector2(width, height);
        }

        public void SetNodeList(List<Node> nodes)
        {
            this.nodes = nodes;
        }
        
        public VisualElement GetElement()
        {
            if (element == null)
            {
                element = new VisualElement();
                element.AddToClassList("node-editor");

                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/Editors/NodeEditor/NodeEditor.uss");
                element.styleSheets.Add(styleSheet);
                
                element.style.width = Size.x;
                element.style.height = Size.y;
                
                element.AddManipulator(this);
            }

            return element;
        }

        public void Clear()
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (nodes[i].HasElement)
                {
                    element.Remove(nodes[i].GetElement());
                }
            }
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
            element.Add(node.GetElement());
        }

        public void RemoveNode(string guid)
        {
            var index = nodes.FindIndex(n => n.serializableGuid == guid);
            if (index >= 0)
            {
                if (nodes[index].HasElement)
                {
                    element.Remove(nodes[index].GetElement());
                }
                nodes.RemoveAt(index);
            }
        }
        
        public void AddContextMenuItem(string label, GenericMenu.MenuFunction2 function) => contextMenu.AddContextMenuItem(label, function);

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
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }
        
        private void OnMouseDown(MouseDownEvent e)
        {
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (e.button == 1)
            {
                contextMenu.ShowContextMenu(e.mousePosition); // TODO account for nodeEditor position, which is not at 0,0
                e.StopPropagation();
            }
        }
        
    }
}