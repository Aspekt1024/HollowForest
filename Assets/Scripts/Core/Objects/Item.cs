using HollowForest.Events;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest.Objects
{
    public class Item : MonoBehaviour, IInteractive
    {
        public GameplayEvent gameplayEvent;
        
        public void OnInteract(Character character)
        {
            
        }

        public void OnOverlap(Character character)
        {
            Game.Events.EventAchieved(gameplayEvent.eventID);
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