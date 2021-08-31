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
            node.Init(this);
            
            var serializedNode = GetNode(new Guid(node.serializableGuid));
            if (serializedNode != null)
            {
                node.SetBaseData(serializedNode);
                RemoveNode(serializedNode);
            }
            
            nodes.Add(node);
            element.Add(node.GetElement());
        }

        public void RemoveNode(Node node)
        {
            if (nodes.Contains(node))
            {
                nodes.Remove(node);
            }
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
    }
}