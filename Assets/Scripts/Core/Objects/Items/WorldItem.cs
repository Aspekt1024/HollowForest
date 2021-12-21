using HollowForest.Events;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest.Objects
{
    public class WorldItem : MonoBehaviour, IInteractive
    {
        public ItemRef item;

        private Item itemData;

        public void Init()
        {
            itemData = Game.Objects.GetItemData(item);
            if (itemData == null) return;
            if (itemData.IsCollected(Game.Characters.GetPlayerCharacter()))
            {
                Destroy(gameObject);
            }
        }
        
        public void OnInteract(Character character)
        {
            
        }

        public void OnOverlap(Character character)
        {
            itemData.Collect(character);
            Game.Events.EventAchieved(itemData.collectionEvent.eventID);
            Destroy(gameObject);
        }

        public void OnOverlapEnd(Character character)
        {
        }

        public InteractiveOverlayDetails GetOverlayDetails()
        {
            return InteractiveOverlayDetails.None;
        }
    }
}