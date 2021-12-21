using System;
using HollowForest.Events;
using HollowForest.Objects;

namespace HollowForest.Shop
{
    [Serializable]
    public class ShopItem
    {
        public ItemRef item;
        public string description;
        public int cost;
        public GameplayEvent requiredEvent;

        public bool IsAvailable()
        {
            return Game.Events.IsAchieved(requiredEvent);
        }
    }
}