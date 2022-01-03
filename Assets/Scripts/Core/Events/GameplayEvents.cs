using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HollowForest.Events
{
    public class GameplayEvents
    {
        private readonly Data.Data data;
        
        public interface IObserver
        {
            void OnEventAchieved(GameplayEvent gameplayEvent);
        }

        private readonly List<IObserver> observers = new List<IObserver>();
        private readonly Dictionary<int, List<Action<GameplayEvent>>> eventObservers = new Dictionary<int, List<Action<GameplayEvent>>>();

        public GameplayEvents(Data.Data data)
        {
            this.data = data;
        }

        public void EventAchieved(GameplayEvent gameplayEvent)
        {
            if (gameplayEvent.eventID <= 0) return;
            EventAchieved(gameplayEvent.eventID);
        }

        public bool IsAchieved(GameplayEvent gameplayEvent)
        {
            if (gameplayEvent.eventID <= 0) return true;
            return data.GameData.achievedEvents.Contains(gameplayEvent.eventID);
        }

        public void EventAchieved(int eventID)
        {
            var index = data.Config.events.FindIndex(e => e.eventID == eventID);
            if (index < 0) return;
            
            if (data.GameData.achievedEvents.Contains(eventID)) return;
            data.GameData.achievedEvents.Add(eventID);
            
            observers.ForEach(o => o.OnEventAchieved(data.Config.events[index]));

            if (eventObservers.ContainsKey(eventID))
            {
                eventObservers[eventID].ToList().ForEach(o => o.Invoke(data.Config.events[index]));
            }
        }
        
        public void RegisterObserver(IObserver observer) => observers.Add(observer);
        public void UnregisterObserver(IObserver observer) => observers.Remove(observer);

        public void RegisterEventObserver(GameplayEvent gameplayEvent, Action<GameplayEvent> callback)
        {
            if (!eventObservers.ContainsKey(gameplayEvent.eventID))
            {
                eventObservers.Add(gameplayEvent.eventID, new List<Action<GameplayEvent>>());
            }
            eventObservers[gameplayEvent.eventID].Add(callback);
        }

        public void UnregisterEventObserver(GameplayEvent gameplayEvent, Action<GameplayEvent> callback)
        {
            if (!eventObservers.ContainsKey(gameplayEvent.eventID)) return;
            eventObservers[gameplayEvent.eventID].Remove(callback);

            if (!eventObservers[gameplayEvent.eventID].Any())
            {
                eventObservers.Remove(gameplayEvent.eventID);
            }
        }
    }
}