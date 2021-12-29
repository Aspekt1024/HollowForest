using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace HollowForest.AI
{
    [Serializable]
    public class LogFilter
    {
        public List<CharacterStates> suppressedCharStates = new List<CharacterStates>();
        public List<AIState> suppressedAItates = new List<AIState>();
        public List<AIObject> suppressedAIObjects = new List<AIObject>();

        public event Action OnUpdated = delegate { };
        
        public void Populate(VisualElement element)
        {
            element.Add(CreateFilterHeader("Log Filter"));
            
            element.Add(CreateFilterHeader("Suppressed Character States"));
            foreach (var state in suppressedCharStates)
            {
                var attributeDisplay = CreateAttributeDisplay(state, () => AllowAttribute(state));
                element.Add(attributeDisplay);
            }
            
            element.Add(CreateFilterHeader("Suppressed AI States"));
            foreach (var state in suppressedAItates)
            {
                var attributeDisplay = CreateAttributeDisplay(state, () => AllowAttribute(state));
                element.Add(attributeDisplay);
            }
            
            element.Add(CreateFilterHeader("Suppressed AI Objects"));
            foreach (var state in suppressedAIObjects)
            {
                var attributeDisplay = CreateAttributeDisplay(state, () => AllowAttribute(state));
                element.Add(attributeDisplay);
            }
        }
        
        public void SuppressAttribute(CharacterStates charState)
        {
            if (suppressedCharStates.Contains(charState)) return;
            suppressedCharStates.Add(charState);
            OnUpdated?.Invoke();
        }
        public void AllowAttribute(CharacterStates charState)
        {
            suppressedCharStates.Remove(charState);
            OnUpdated?.Invoke();
        }
        
        public void SuppressAttribute(AIState aiState)
        {
            if (suppressedAItates.Contains(aiState)) return;
            suppressedAItates.Add(aiState);
            OnUpdated?.Invoke();
        }
        public void AllowAttribute(AIState aiState)
        {
            suppressedAItates.Remove(aiState);
            OnUpdated?.Invoke();
        }

        public void SuppressAttribute(AIObject aiObject)
        {
            if (suppressedAIObjects.Contains(aiObject)) return;
            suppressedAIObjects.Add(aiObject);
            OnUpdated?.Invoke();
        }
        public void AllowAttribute(AIObject aiObject)
        {
            suppressedAIObjects.Remove(aiObject);
            OnUpdated?.Invoke();
        }

        private VisualElement CreateFilterHeader(string label)
        {
            var header = new Label(label);
            header.AddToClassList("inspector-header");
            return header;
        }

        private VisualElement CreateAttributeDisplay(Enum attribute, Action onClickCallback)
        {
            var display = new Button { text = attribute.ToString() };
            display.AddToClassList("attribute-display");
            display.clicked += () =>
            {
                onClickCallback?.Invoke();
                OnUpdated?.Invoke();
            };

            return display;
        }
    }
}