using UnityEngine;
using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public class ConnectionElement : VisualElement
    {
        private readonly Node output;
        private readonly Node input;
        
        private readonly NodeEditor nodeEditor;
        public readonly int DependencyTypeID;

        private Vector2 mousePos;
        
        /// <summary>
        /// Creates a connection between two nodes
        /// </summary>
        public ConnectionElement(Node output, Node input, Node.DependencyProfile dependencyProfile)
        {
            this.output = output;
            this.input = input;
            DependencyTypeID = dependencyProfile.dependencyTypeID;
            
            generateVisualContent = (ctx) => DrawLine(
                ctx,
                output.GetConnectingPosition(input.GetPosition()),
                input.GetConnectingPosition(output.GetPosition()),
                dependencyProfile.lineColor,
                dependencyProfile.lineThickness,
                dependencyProfile.isGlowEnabled
            );

            output.OnMove += OnAttachedNodeMoved;
            input.OnMove += OnAttachedNodeMoved;
        }

        /// <summary>
        /// Creates a connection element from one node to the mouse position.
        /// This is a temporary connection element used while a dependency is being setup.
        /// </summary>
        public ConnectionElement(NodeEditor nodeEditor, Node outputNode, Node.DependencyProfile dependencyProfile)
        {
            this.nodeEditor = nodeEditor;
            
            DependencyTypeID = dependencyProfile.dependencyTypeID;

            mousePos = outputNode.GetConnectingPosition(outputNode.GetPosition());
            
            generateVisualContent = ctx =>
            {
                DrawLine(
                    ctx,
                    outputNode.GetConnectingPosition(mousePos),
                    mousePos,
                    dependencyProfile.lineColor,
                    dependencyProfile.lineThickness,
                    dependencyProfile.isGlowEnabled
                );
            };

            nodeEditor.OnDrag += NodeEditorMouseMoved;
        }

        public bool IsConnectedToNode(Node node) => input == node || output == node;
        public Node Input => input;
        public Node Output => output;
        
        private void DrawLine(MeshGenerationContext ctx, Vector2 startPos, Vector2 endPos, Color color, float thickness, bool isGlowEnabled)
        {
            if (isGlowEnabled)
            {
                var colorBase = new Color(color.r, color.g, color.b, color.a * 0.3f);
                var colorNear = new Color(color.r, color.g, color.b, color.a * 0.5f);
                DrawLine(ctx, startPos, endPos, colorBase, thickness * 3f);
                DrawLine(ctx, startPos, endPos, colorNear, thickness * 1.5f);
            }
            
            DrawLine(ctx, startPos, endPos, color, thickness);
            DrawTriangle(ctx, startPos, endPos, color, thickness);
        }

        private void DrawLine(MeshGenerationContext ctx, Vector2 startPos, Vector2 endPos, Color color, float thickness)
        {
            var mesh = ctx.Allocate( 6, 6 );
            
            var dir = (endPos - startPos).normalized;

            var startCenter = new Vector3(startPos.x, startPos.y, Vertex.nearZ);
            var endCenter = new Vector3(endPos.x, endPos.y, Vertex.nearZ);
            var normal = new Vector3(-dir.y, dir.x, 0f);

            var halfThickness = thickness * 0.5f;
            
            var vertices = new Vertex[6];
            vertices[0].position = startCenter + normal * halfThickness;
            vertices[1].position = startCenter - normal * halfThickness;
            vertices[2].position = endCenter + normal * halfThickness;
            
            vertices[3].position = startCenter - normal * halfThickness;
            vertices[4].position = endCenter - normal * halfThickness;
            vertices[5].position = endCenter + normal * halfThickness;
            
            vertices[0].tint = color;
            vertices[1].tint = color;
            vertices[2].tint = color;
            vertices[3].tint = color;
            vertices[4].tint = color;
            vertices[5].tint = color;
            
            mesh.SetAllVertices( vertices );
            mesh.SetAllIndices( new ushort[]{ 0, 1, 2, 3, 4, 5 });
        }

        private void DrawTriangle(MeshGenerationContext ctx, Vector2 startPos, Vector2 endPos, Color color, float thickness)
        {
            var mesh = ctx.Allocate( 3, 3 );

            var length = thickness * 4f;
            var width = thickness * 3f;

            var distAlongLine = 1f;
            
            var dir = (endPos - startPos).normalized;
            var dist = (endPos - startPos).magnitude;
            var center = new Vector3(startPos.x + dir.x * (dist * distAlongLine - length), startPos.y + dir.y * (dist * distAlongLine -  length), Vertex.nearZ);
            var normal = new Vector3(-dir.y, dir.x, 0f);
            
            var vertices = new Vertex[3];
            vertices[0].position = center + normal * width;
            vertices[1].position = center - normal * width;
            vertices[2].position = new Vector3(center.x + dir.x * length, center.y + dir.y * length, center.z);
            
            vertices[0].tint = color;
            vertices[1].tint = color;
            vertices[2].tint = color;
            
            mesh.SetAllVertices( vertices );
            mesh.SetAllIndices(new ushort[] { 0, 1, 2 });
        }

        ~ConnectionElement()
        {
            if (output != null) output.OnMove -= OnAttachedNodeMoved;
            if (input != null) input.OnMove -= OnAttachedNodeMoved;
            if (nodeEditor != null) nodeEditor.OnDrag -= NodeEditorMouseMoved;
        }

        private void NodeEditorMouseMoved(Vector2 mousePos)
        {
            this.mousePos = mousePos;
            MarkDirtyRepaint();
        }

        private void OnAttachedNodeMoved(Node node)
        {
            MarkDirtyRepaint();
        }
    }
}