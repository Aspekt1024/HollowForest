using UnityEngine;

namespace HollowForest.Objects
{
    public class Door : Switchable
    {
        private Animator anim;

        private static readonly int OpenAnim = Animator.StringToHash("Open");
        private static readonly int CloseAnim = Animator.StringToHash("Close");
        private static readonly int OpenAnimBool = Animator.StringToHash("isOpen");

        protected override void Awake()
        {
            base.Awake();
            
            anim = GetComponent<Animator>();
            anim.Play(IsSwitchedOn ? OpenAnim : CloseAnim, 0, 1f);
            anim.SetBool(OpenAnimBool, IsSwitchedOn);
        }
        protected override void OnSwitchedOn()
        {
            anim.SetBool(OpenAnimBool, true);
            Game.GetInstance().cameraManager.FollowTemporary(transform, 1.5f);
        }

        protected override void OnSwitchedOff()
        {
            anim.SetBool(OpenAnimBool, false);
        }
    }
}