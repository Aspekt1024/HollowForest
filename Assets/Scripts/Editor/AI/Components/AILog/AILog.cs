using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    public class AILog : VisualElement
    {
        private readonly LogFilter filter;
        private readonly VisualElement logContainer;
        private readonly ScrollView messageContainer;
        
        private AIAgent agent;

        private readonly List<LogMessage> messages = new List<LogMessage>();

        public AILog(LogFilter filter)
        {
            this.filter = filter;
            
            logContainer = new VisualElement();
            logContainer.AddToClassList("log-view");
            messageContainer = new ScrollView(ScrollViewMode.Vertical);
            logContainer.Add(messageContainer);
        }

        public void Init(VisualElement page)
        {
            page.Add(logContainer);
        }

        public void Populate()
        {
            InitialiseLogView();

            foreach (var message in messages)
            {
                AppendMessageToLogView(message);
            }
        }

        public void SetAgent(AIAgent newAgent)
        {
            if (agent == newAgent) return;
            if (agent != null)
            {
                UnregisterCallbacks(agent);
            }
            
            messages.Clear();
            InitialiseLogView();

            agent = newAgent;
            RegisterCallbacks(agent);
        }

        private void RegisterCallbacks(AIAgent a)
        {
            a.character.State.RegisterAllStateObserver(OnCharacterStateChanged);
            a.memory.RegisterAllStateObserver(OnAIStateChanged);
            a.memory.RegisterAllObjectObserver(OnAIObjectChanged);
            a.RegisterActionObserver(OnAIActionChanged);
        }

        private void UnregisterCallbacks(AIAgent a)
        {
            a.character.State.UnregisterAllStateObserver(OnCharacterStateChanged);
            a.memory.UnregisterAllStateObserver(OnAIStateChanged);
            a.memory.UnregisterAllObjectObserver(OnAIObjectChanged);
            a.UnregisterActionObserver(OnAIActionChanged);
        }

        private void OnCharacterStateChanged(CharacterStates state, bool value)
        {
            AddMessageEntry(new CharStateMessage(state, value, filter));
        }

        private void OnAIStateChanged(AIState state, bool value)
        {
            AddMessageEntry(new AIStateMessage(state, value, filter));
        }

        private void OnAIObjectChanged(AIObject state, object value)
        {
            AddMessageEntry(new AIObjectMessage(state, value, filter));
        }

        private void OnAIActionChanged(AIExecutor.TransitionInfo info)
        {
            AddMessageEntry(new AIActionMessage(info, filter));
        }

        private void AddMessageEntry(LogMessage message)
        {
            messages.Add(message);
            AppendMessageToLogView(message);
        }
        
        // TODO ai action change (via interrupt / transition)

        private void InitialiseLogView()
        {
            logContainer.Clear();
            messageContainer.Clear();
            
            var header = new VisualElement();
            header.AddToClassList("log-message-container");
            header.AddToClassList("log-header-container");

            var timestampHeader = new Label("Time");
            timestampHeader.AddToClassList("log-timestamp");
            timestampHeader.AddToClassList("log-header-text");
            header.Add(timestampHeader);
            
            var messageHeader = new Label("Message");
            messageHeader.AddToClassList("log-message");
            messageHeader.AddToClassList("log-header-text");
            header.Add(messageHeader);
            
            logContainer.Add(header);
            logContainer.Add(messageContainer);
        }
        
        private void AppendMessageToLogView(LogMessage message)
        {
            if (message.IsAllowedByFilter(filter))
            {
                var msgLine = message.GetElement();
                messageContainer.Add(msgLine);
            }
        }
    }
}