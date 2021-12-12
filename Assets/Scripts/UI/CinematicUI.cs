using System;
using UnityEngine;

namespace HollowForest.UI
{
    public class CinematicUI : UIBase
    {
        private Animator anim;

        private static readonly int HIddenAnim = Animator.StringToHash("Hidden");
        private static readonly int IsActiveBool = Animator.StringToHash("isActive");

        private bool isActive;
        private float timeToDisable;

        protected override void OnAwake()
        {
            anim = GetComponent<Animator>();
        }
        
        public override void OnAcceptPressed()
        {
            
        }

        public void Activate(float duration)
        {
            isActive = true;
            timeToDisable = Time.unscaledTime + duration;
            Show();
        }

        private void Update()
        {
            if (isActive)
            {
                if (Time.unscaledTime >= timeToDisable)
                {
                    isActive = false;
                    Hide();
                }
            }
        }

        protected override bool OnShow()
        {
            anim.SetBool(IsActiveBool, true);
            return true;
        }

        protected override bool OnHide()
        {
            anim.SetBool(IsActiveBool, false);
            return true;
        }

        protected override void OnHideImmediate()
        {
            anim.Play(HIddenAnim, 0, 1f);
        }
    }
}