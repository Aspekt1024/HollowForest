using System;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest.Events
{
    public class WorldEventTriggerPoint : MonoBehaviour, IInteractive
    {
        private WorldEvent worldEvent;
        
        public void Init(WorldEvent worldEvent)
        {
            this.worldEvent = worldEvent;
        }

        public void OnInteract(Character character)
        {
            
        }

        public void OnOverlap(Character character)
        {
            worldEvent.OnCharacterTriggered(character);
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