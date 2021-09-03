using System;
using System.Collections.Generic;
using HollowForest.Dialogue;

namespace HollowForest.Data
{
    [Serializable]
    public class GameData
    {
        public List<int> achievedEvents = new List<int>();
        public DialogueData dialogue = new DialogueData();
    }
}