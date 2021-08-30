using HollowForest.Dialogue;

namespace HollowForest.Events
{
    public class GameplayEvents
    {
        private Data.Data data;

        public GameplayEvents(Data.Data data)
        {
            this.data = data;
        }

        public void GameplayEventAchieved(GameplayEvent e)
        {
            if (data.GameData.achievedGameplayEvents.Contains(e)) return;
            data.GameData.achievedGameplayEvents.Add(e);
        }
        
        public void DialogueEventAchieved(DialogueEvent e)
        {
            if (data.GameData.achievedDialogueEvents.Contains(e)) return;
            data.GameData.achievedDialogueEvents.Add(e);
        }
    }
}