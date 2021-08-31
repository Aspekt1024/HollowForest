using System;
using System.Collections.Generic;

namespace HollowForest.Data
{
    [Serializable]
    public class GameData
    {
        public List<int> achievedEvents;

        public GameData()
        {
            achievedEvents = new List<int>();
        }
    }
}