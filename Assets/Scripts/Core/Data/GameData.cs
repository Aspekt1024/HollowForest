using System;
using System.Collections.Generic;

namespace HollowForest.Data
{
    [Serializable]
    public class GameData
    {
        public List<int> achievedEvents;
        public List<string> completedDialogue;

        public GameData()
        {
            achievedEvents = new List<int>();
            completedDialogue = new List<string>();
        }
    }
}