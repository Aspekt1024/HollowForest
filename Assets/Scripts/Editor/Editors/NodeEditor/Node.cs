using System;
using System.Collections.Generic;
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
            public string selectedStyle;
            public string unselectedStyle;
            public string activatingLink;
        }

        public struct DependencyProfile
        {
            public int dependencyTypeID;
            public Color lineColor;
            public float lineThickness;
            public bool isGlowEnabled;

            public DependencyProfile(int dependencyTypeID, Color lineColor)
            {
                this.dependencyTypeID = dependencyTypeID;
                this.lineColor = lineColor;
                lineThickness = 1.5f;
                isGlowEnabled = true;
            }
        }

        private VisualElement element;
        protected NodeEditor Parent;

        private bool isSelected;
        private bool isMouseDown;
        private Vector2 rawPosition;

        public event Action<Node> OnMove = delegate { };
        public event Action<Node> OnEnter = delegate { };
        public event Action<Node> OnLeave = delegate { };
        public event Action<Node> OnClick = delegate { };
        
        private readonly ContextMenu contextMenu;
        private readonly List<DependencyProfile> dependencyProfiles;
        
        private StyleProfile styles;
        
        public bool HasElement => element != null;
        public Vector2 GetPosition() => position;
        public Vector2 GetSize() => size;

        protected Node(Guid guid, List<DependencyProfile> dependencyProfiles)
        {
            this.dependencyProfiles = dependencyProfiles;
            
            contextMenu = new ContextMenu();
            serializableGuid = guid.ToString();
        }

        public void Init(NodeEditor parent)
        {
            Parent = parent;
            GetElement();
        }

        protected virtual void Populate(VisualElement element) {}
        public virtual void PopulateInspector(VisualElement container) {}
        
        public virtual bool CreateDependency(Node dependency) { return false; }
        public virtual bool RemoveDependency(Node dependency) { return false; }

        public VisualElement GetElement()
        {
            if (element == null)
            {
                element = new VisualElement();
                element.AddToClassList("node");

                SetBaseData(this);

                Populate(element);
                
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

        public DependencyProfile GetDependencyProfile(int id)
        {
            var index = dependencyProfiles.FindIndex(p => p.dependencyTypeID == id);
            if (index < 0)
            {
                return new DependencyProfile
                {
                    dependencyTypeID = 0,
                    lineColor = new Color(0.28f, 0.65f, 1f),
                    lineThickness = 1.5f,
                };
            }

            return dependencyProfiles[index];
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
            rawPosition = newPos;

            position.x = Mathf.Round(rawPosition.x / NodeEditor.GridStepSize) * NodeEditor.GridStepSize;
            position.y = Mathf.Round(rawPosition.y / NodeEditor.GridStepSize) * NodeEditor.GridStepSize;
            
            element.style.left = position.x;
            element.style.top = position.y;
        }

        protected void SetStyle(StyleProfile styleProfile)
        {
            if (!string.IsNullOrEmpty(styles.activatingLink))
            {
                element.RemoveFromClassList(styles.unselectedStyle);
                element.RemoveFromClassList(styles.selectedStyle);
                element.RemoveFromClassList(styles.activatingLink);
            }

            styles = styleProfile;
            SetSelectedState();
        }
        
        protected void SetSize(Vector2 newSize)
        {
            size = newSize;

            element.style.width = size.x;
            element.style.height = size.y;
        }

        public void SetSelectedState()
        {
            if (isSelected)
            {
                ShowSelected();
            }
            else
            {
                ShowUnselected();
            }
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
        
        public virtual void ActivatingLinkStart()
        {
            GetElement().AddToClassList(styles.activatingLink);
        }

        public virtual void ActivatingLinkEnd()
        {
            GetElement().RemoveFromClassList(styles.activatingLink);
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseMoveEvent>(OnMouseMoved);
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            target.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMoved);
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
            target.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        private void OnMouseMoved(MouseMoveEvent e)
        {
            if (isMouseDown && e.button == 0)
            {
                var delta = e.mouseDelta / Parent.Zoom;
                SetPosition(rawPosition + delta);
                OnMove?.Invoke(this);
            }
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if (e.button == 0)
            {
                isMouseDown = true;
                target.CaptureMouse();
                e.StopPropagation();

                ShowSelected();
                OnClick?.Invoke(this);
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

        private void OnMouseEnter(MouseEnterEvent e)
        {
            OnEnter?.Invoke(this);
        }

        private void OnMouseLeave(MouseLeaveEvent e)
        {
            OnLeave?.Invoke(this);
        }
    }
}