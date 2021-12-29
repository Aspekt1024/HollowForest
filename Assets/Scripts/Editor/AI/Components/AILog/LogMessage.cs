using System;
using Aspekt.Editors;
using UnityEditor;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public abstract class LogMessage : MouseManipulator
    {
        public enum Severity
        {
            Info = 100,
            Warning = 200,
            Error = 300,
        }
        
        public string timeStamp;
        public Severity severity;
        
        private readonly ContextMenu contextMenu;
        private VisualElement element;

        private readonly LogFilter filter;

        protected LogMessage(LogFilter filter, Severity severity)
        {
            this.filter = filter;
            
            this.severity = severity;
            timeStamp = DateTime.Now.ToString("hh:mm:ss.fff");
            
            contextMenu = new ContextMenu();
        }

        public abstract bool IsAllowedByFilter(LogFilter filter);

        public VisualElement GetElement()
        {
            element = new VisualElement();
            element.AddToClassList("log-message-container");

            var timestamp = new Label(timeStamp);
            timestamp.AddToClassList("log-timestamp");
            element.Add(timestamp);
            
            var logMessage = new Label(GetMessage());
            logMessage.AddToClassList("log-message");
            if (!string.IsNullOrEmpty(AdditionalMessageStyle))
            {
                logMessage.AddToClassList(AdditionalMessageStyle);
            }
            if (severity == Severity.Warning)
            {
                logMessage.AddToClassList("log-message-warning");
            }
            else if (severity == Severity.Error)
            {
                logMessage.AddToClassList("log-message-error");
            }
            
            element.Add(logMessage);
            element.AddManipulator(this);
            
            contextMenu.ClearContextMenuItems();
            SetupContextMenu(filter);
            
            return element;
        }

        protected virtual string AdditionalMessageStyle => "";
        
        protected abstract string GetMessage();

        protected abstract void SetupContextMenu(LogFilter filter);

        protected void AddContextMenuItem(string label, GenericMenu.MenuFunction2 function)
        {
            contextMenu.AddContextMenuItem(label, function);
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (e.button == 1)
            {
                contextMenu.ShowContextMenu(e.mousePosition);
                e.StopPropagation();
            }
        }
    }
}