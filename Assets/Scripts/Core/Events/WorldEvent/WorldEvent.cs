using System;
using UnityEngine;

namespace HollowForest.Events
{
    public class WorldEvent : MonoBehaviour
    {
        public WorldEventTriggerPoint triggerPoint;
        public GameplayEvent startGameplayEvent;
        public bool hasCinematic;
        public float cinematicDuration;
        public bool centerCameraInZone;
        public bool centerCameraAfterCompletion;

        private float eventBeginTime;
        private bool isAwaitingBegin;
        
        private bool isTriggered;
        private bool isEnabled;
        private bool isComplete;

        private EventBehaviour[] events;
        private BoxCollider2D coll;

        private void Awake()
        {
            coll = GetComponent<BoxCollider2D>();
            if (triggerPoint != null)
            {
                triggerPoint.Init(this);
            }
            
            events = GetComponentsInChildren<EventBehaviour>();
        }

        public void OnCharacterTriggered(Character character)
        {
            if (!character.IsPlayer()) return;
            
            if (!isTriggered)
            {
                isEnabled = true;
                isTriggered = true;
                
                eventBeginTime = Time.time;
                isAwaitingBegin = true;
                if (hasCinematic)
                {
                    Game.Camera.SetCinematic(cinematicDuration);
                    eventBeginTime += cinematicDuration;
                }
                
                Game.Events.EventAchieved(startGameplayEvent.eventID);
            
                foreach (var eventBehaviour in events)
                {
                    eventBehaviour.OnWorldEventTriggered(this, character);
                }
            }
        }

        private void Update()
        {
            if (isAwaitingBegin && Time.time >= eventBeginTime)
            {
                isAwaitingBegin = false;
                BeginEvent();
            }
        }

        public void Complete()
        {
            isEnabled = false;
            isComplete = true;

            if (!centerCameraAfterCompletion)
            {
                Game.Camera.ReleaseBounds();
            }

            foreach (var eventBehaviour in events)
            {
                eventBehaviour.OnWorldEventComplete();
            }
        }
        
        private void BeginEvent()
        {
            foreach (var eventBehaviour in events)
            {
                eventBehaviour.OnWorldEventBegin();
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!CanCenterCamera()) return;
            
            var otherChar = other.GetComponent<Character>();
            if (otherChar != null && otherChar.IsPlayer())
            {
                Game.Camera.ApplyBounds(transform.position, coll.size);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var otherChar = other.GetComponent<Character>();
            if (otherChar != null && otherChar.IsPlayer())
            {
                Game.Camera.ReleaseBounds();
            }
        }

        private bool CanCenterCamera()
        {
            if (isComplete && !centerCameraAfterCompletion) return false;
            return centerCameraInZone;
        }
    }
}