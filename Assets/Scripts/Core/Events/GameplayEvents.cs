using System.Collections.Generic;
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

        public void RegisterObserver(IObserver observer) => observers.Add(observer);
        public void UnregisterObserver(IObserver observer) => observers.Remove(observer);

        public GameplayEvents(Data.Data data)
        {
            this.data = data;
        }

        public void EventAchieved(int eventID)
        {
            var index = data.Config.events.FindIndex(e => e.eventID == eventID);
            if (index < 0)
            {
                Debug.LogError($"Invalid event id: {eventID}");
                return;
            }
            
            if (data.GameData.achievedEvents.Contains(eventID)) return;
            data.GameData.achievedEvents.Add(eventID);
            
            observers.ForEach(o => o.OnEventAchieved(data.Config.events[index]));
        }
    }
}