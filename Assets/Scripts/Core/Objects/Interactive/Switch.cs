using System.Collections.Generic;
using HollowForest.Events;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest.Objects
{
    public class Switch : Switchable, IInteractive
    {
        public List<Switchable> switchedObjects;
        public GameplayEvent gameplayEvent;
        
        private Animator anim;

        private static readonly int OnAnim = Animator.StringToHash("On");
        private static readonly int OffAnim = Animator.StringToHash("Off");
        private static readonly int OnAnimBool = Animator.StringToHash("isOn");

        protected override void Awake()
        {
            base.Awake();
            
            anim = GetComponent<Animator>();
            anim.Play(IsSwitchedOn ? OnAnim : OffAnim, 0, 1f);
        }

        public void OnInteract(Character character)
        {
            Switch();
        }

        public void OnOverlap(Character character) { }
        public void OnOverlapEnd(Character character) { }

        protected override void OnSwitchedOn()
        {
            anim.SetBool(OnAnimBool, true);
            switchedObjects.ForEach(o => o.SwitchOn());
        }

        protected override void OnSwitchedOff()
        {
            anim.SetBool(OnAnimBool, false);
            switchedObjects.ForEach(o => o.SwitchOff());
        }
    }
}