using System.Collections.Generic;
using HollowForest.Objects;
using UnityEngine;

namespace HollowForest.Events
{
    public class SwitchedObjectEvent : EventBehaviour
    {
        public List<Switchable> switchedObjects;
        public float switchDelay;

        private bool isAwaitingSwitch;
        private float timeToSwitch;

        public override void OnWorldEventTriggered(WorldEvent worldEvent, Character character)
        {
            switchedObjects.ForEach(s => s.SwitchOff());
        }

        public override void OnWorldEventBegin()
        {
        }

        public override void OnWorldEventComplete()
        {
            timeToSwitch = Time.time + switchDelay;
            isAwaitingSwitch = true;
        }

        private void Update()
        {
            if (isAwaitingSwitch && Time.time >= timeToSwitch)
            {
                isAwaitingSwitch = false;
                switchedObjects.ForEach(s => s.SwitchOn());
            }
        }
    }
}