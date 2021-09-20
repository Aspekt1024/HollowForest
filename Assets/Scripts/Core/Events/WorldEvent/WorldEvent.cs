using System.Collections.Generic;
using HollowForest.Objects;
using UnityEngine;

namespace HollowForest.Events
{
    public class WorldEvent : MonoBehaviour
    {
        public WorldEventTriggerPoint triggerPoint;
        public GameplayEvent startGameplayEvent;
        public GameplayEvent endGameplayEvent;
        public bool centerCameraInZone;

        public List<Switchable> switchedObjects;

        private bool isTriggered;
        private bool isEnabled;

        private BoxCollider2D coll;

        private void Awake()
        {
            coll = GetComponent<BoxCollider2D>();
            triggerPoint.Init(this);
        }

        public void OnCharacterTriggered(Character character)
        {
            if (!isTriggered)
            {
                isEnabled = true;
                isTriggered = true;
                BeginEvent();
            }
        }
        
        private void BeginEvent()
        {
            Game.Events.EventAchieved(startGameplayEvent.eventID);
            switchedObjects.ForEach(s => s.SwitchOff());
            Game.Characters.BlockInput(2f);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!centerCameraInZone) return;
            
            var otherChar = other.GetComponent<Character>();
            if (otherChar != null)
            {
                Game.Camera.ApplyBounds(transform.position, coll.size);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!centerCameraInZone) return;

            var otherChar = other.GetComponent<Character>();
            if (otherChar != null)
            {
                Game.Camera.ReleaseBounds();
            }
        }
    }
}