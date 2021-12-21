using System;
using HollowForest.Events;

namespace HollowForest.Objects
{
    /// <summary>
    /// An instance of an item used in the world (world item, shop item, etc).
    /// </summary>
    [Serializable]
    public class Item
    {
        public int id;
        public string name;
        public string description;
        public GameplayEvent collectionEvent;
        public ItemBehaviour behaviour;

        public bool IsCollected(Character character) => behaviour.IsCollected(this, character);
        public void Collect(Character character) => behaviour.Collect(this, character);
    }

    /// <summary>
    /// A reference to an <see cref="Item"/>
    /// </summary>
    [Serializable]
    public class ItemRef
    {
        public int id;
    }
}