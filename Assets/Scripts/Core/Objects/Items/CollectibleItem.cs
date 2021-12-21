
using UnityEngine;

namespace HollowForest.Objects
{
    public enum CollectibleItemType
    {
        StoryItem = 0, // For items that are only attached to an event
    }

    public enum CollectibleItemCategory
    {
        None = 0, // Collected, but not displayed in the inventory
    }
    
    /// <summary>
    /// An item that can be added to the player's inventory
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Collectible Item", fileName = "NewCollectibleItem")]

    public class CollectibleItem : ItemBehaviour
    {
        public CollectibleItemType itemType;
        
        
        protected override void OnCollected(Item item, Character character)
        {
            // TODO add item to inventory
        }

        public override bool IsCollected(Item item, Character character)
        {
            // TODO potentially allow multiple items to be collected for the same event
            return Game.Events.IsAchieved(item.collectionEvent);
        }
    }
}