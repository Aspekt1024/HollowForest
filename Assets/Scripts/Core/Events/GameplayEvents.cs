using UnityEngine;

namespace HollowForest.Events
{
    public class GameplayEvents
    {
        private readonly Data.Data data;

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
        }

        public void DialogueComplete(string dialogueGuid)
        {
            if (data.GameData.completedDialogue.Contains(dialogueGuid)) return;
            data.GameData.completedDialogue.Add(dialogueGuid);
        }
    }
}