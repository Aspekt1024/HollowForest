using System;
using UnityEngine;

namespace HollowForest.Objects
{
    public abstract class Switchable : MonoBehaviour
    {
        public bool startOn;
        
        protected bool IsSwitchedOn;

        protected virtual void Awake()
        {
            IsSwitchedOn = startOn;
        }

        public void Switch()
        {
            if (IsSwitchedOn)
            {
                SwitchOff();
            }
            else
            {
                SwitchOn();
            }
        }

        public void SwitchOn()
        {
            IsSwitchedOn = true;
            OnSwitchedOn();
        }

        public void SwitchOff()
        {
            IsSwitchedOn = false;
            OnSwitchedOff();
        }

        protected abstract void OnSwitchedOn();
        protected abstract void OnSwitchedOff();
    }
}