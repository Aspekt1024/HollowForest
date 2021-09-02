using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    [Serializable]
    public class Node : MouseManipulator
    {
        public string serializableGuid;
        [SerializeField] private Vector2 position;
        [SerializeField] private Vector2 size;

        public struct StyleProfile
        {
            public string baseStyle;
            public string selectedStyle;
            public string unselectedStyle;
        }

        private VisualElement element;
        protected NodeEditor Parent;
        
        private bool isSelected;
        private bool isMouseDown;
        private Vector2 initialMousePos;
        private Vector2 initialNodePos;

        public event Action<Node> OnSelect = delegate { };
        public event Action<Node> OnMove = delegate { };
        
        private readonly ContextMenu contextMenu;
        private readonly StyleProfile styles;
        
        public bool HasElement => element != null;

        protected Node(Guid guid, StyleProfile styles)
        {
            this.styles = styles;
            
            contextMenu = new ContextMenu();
            serializableGuid = guid.ToString();
        }

        public void Init(NodeEditor parent)
        {
            Parent = parent;
            GetElement();
        }

        protected virtual void Populate(VisualElement element) {}

        public VisualElement GetElement()
        {
            if (element == null)
            {
                element = new VisualElement();
                element.AddToClassList("node");
                element.AddToClassList(styles.baseStyle);
                element.AddToClassList(styles.unselectedStyle);

                SetBaseData(this);

                var nodeContent = new VisualElement();
                Populate(nodeContent);
                element.Add(nodeContent);
                
                element.AddManipulator(this);
            }

            return element;
        }
        
        public void AddContextMenuItem(string label, GenericMenu.MenuFunction2 function) => contextMenu.AddContextMenuItem(label, function);

        public void SetBaseData(Node data)
        {
            serializableGuid = data.serializableGuid;
            SetPosition(data.position);
            SetSize(data.size);
        }

        public virtual Vector2 GetConnectingPosition(Vector2 fromPos)
        {
            var e = element;
            var pos = position;

            var dist = pos - fromPos;
            if (Mathf.Abs(dist.y) > Mathf.Abs(dist.x))
            {
                pos.x += e.layout.width / 2f;

                if (fromPos.y > pos.y)
                {
                    pos.y += e.layout.height;
                }
            }
            else
            {
                pos.y += e.layout.height / 2f;
            
                if (fromPos.x > pos.x)
                {
                    pos.x += e.layout.width;
                }
            }

            return pos;
        }

        public void SetPosition(Vector2 newPos)
        {
            position = newPos;
            position.x = Mathf.Clamp(position.x, 0, Parent.Size.x - size.x);
            position.y = Mathf.Clamp(position.y, 0, Parent.Size.y - size.y);
            
            element.style.left = position.x;
            element.style.top = position.y;
        }
        
        protected void SetSize(Vector2 newSize)
        {
            size = newSize;

            element.style.width = size.x;
            element.style.height = size.y;
        }

        public void ShowUnselected()
        {
            isSelected = false;
            element?.RemoveFromClassList(styles.selectedStyle);
            element?.AddToClassList(styles.unselectedStyle);
        }

        public void ShowSelected()
        {
            isSelected = true;
            element?.AddToClassList(styles.selectedStyle);
            element?.RemoveFromClassList(styles.unselectedStyle);
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseMoveEvent>(OnMouseMoved);
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMoved);
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseMoved(MouseMoveEvent e)
        {
            if (isMouseDown && e.button == 0)
            {
                var delta = e.mousePosition - initialMousePos;
                SetPosition(initialNodePos + delta);
                OnMove?.Invoke(this);
            }
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if (e.button == 0)
            {
                isMouseDown = true;
                initialMousePos = e.mousePosition;
                initialNodePos = position;
                target.CaptureMouse();
                e.StopPropagation();

                ShowSelected();
                OnSelect?.Invoke(this);
            }
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (e.button == 0 && isMouseDown)
            {
                isMouseDown = false;
                target.ReleaseMouse();
                e.StopPropagation();
            }
            else if (e.button == 1)
            {
                contextMenu.ShowContextMenu(e.mousePosition);
                e.StopPropagation();
            }
        }
    }
}