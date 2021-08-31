using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public class NodeEditor
    {
        private VisualElement element;

        public Vector2 Size { get; private set; }

        private List<Node> nodes;
        
        public NodeEditor(int width, int height)
        {
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
            }

            return element;
        }

        public void AddNode(Node node)
        {
            var serializedNode = GetNode(node.guid);
            if (serializedNode == null)
            {
                nodes.Add(node);
            }
            else
            {
                node = serializedNode;
            }
            
            node.Init(this);
            element.Add(node.GetElement());
        }

        public void RemoveNode(Node node)
        {
            if (nodes.Contains(node))
            {
                element.Remove(node.GetElement());
                nodes.Remove(node);
            }
        }

        private Node GetNode(Guid guid)
        {
            foreach (var node in nodes)
            {
                if (node.guid == guid)
                {
                    return node;
                }
            }

            return null;
        }
    }
}