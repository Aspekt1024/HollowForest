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
        
        private GameplayEvents events;
        private CameraManager cam;

        private void Awake()
        {
            coll = GetComponent<BoxCollider2D>();
            triggerPoint.Init(this);
        }

        private void Start()
        {
            cam = Game.GetInstance().cameraManager;
            events = Game.GetInstance().events;
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
            events.EventAchieved(startGameplayEvent.eventID);
            
            switchedObjects.ForEach(s => s.SwitchOff());
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!centerCameraInZone) return;
            
            var otherChar = other.GetComponent<Character>();
            if (otherChar != null)
            {
                cam.ApplyBounds(transform.position, coll.size);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!centerCameraInZone) return;

            var otherChar = other.GetComponent<Character>();
            if (otherChar != null)
            {
                cam.ReleaseBounds();
            }
        }
    }
}