using System;

namespace HollowForest.Events
{
    [Serializable]
    public class GameplayEvent
    {
        public int eventID;
        public string eventName;
        public string description;
    }
}