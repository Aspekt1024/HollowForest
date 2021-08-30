using System;
using System.Collections.Generic;
using HollowForest.Dialogue;
using HollowForest.Events;

namespace HollowForest.Data
{
    [Serializable]
    public class GameData
    {
        public List<GameplayEvent> achievedGameplayEvents;
        public List<DialogueEvent> achievedDialogueEvents;

        public GameData()
        {
            achievedGameplayEvents = new List<GameplayEvent>();
            achievedDialogueEvents = new List<DialogueEvent>();
        }
    }
}