using System;
using UnityEngine;

namespace HollowForest.Effects
{
    public class CharacterAnimator
    {
        [Serializable]
        public class Settings
        {
            public Animator animator;
            
            [Header("Animations")]
            public string groundHitLight;
            public string groundHitHeavy;

            [Header("Animation Booleans")]
            public string walking;
            public string running;
            public string falling;
            
            [Header("Animation Triggers")]
            public string lightAttack;
            public string heavyAttack;
            public string cancelAttack;
            public string dashing;
            public string jumping;
        }

        private readonly Animator animator;
        private readonly Transform model;
        private readonly AnimationEventListener animEvents;

        private readonly int groundHitLightAnim;
        private readonly int groundHitHeavyAnim;
        private readonly int lightAttackTrigger;
        private readonly int heavyAttackTrigger;
        private readonly int cancelAttackTrigger;
        
        private readonly int dashTrigger;
        private readonly int jumpTrigger;
        
        private readonly int walkBool;
        private readonly int runBool;
        private readonly int fallBool;

        public CharacterAnimator(Character character, Settings settings, Transform model)
        {
            animator = settings.animator;
            this.model = model;
            animEvents = character.GetComponentInChildren<AnimationEventListener>();

            groundHitLightAnim = GetAnimationHash(settings.groundHitLight);
            groundHitHeavyAnim = GetAnimationHash(settings.groundHitHeavy);

            walkBool = GetAnimationHash(settings.walking);
            runBool = GetAnimationHash(settings.running);
            fallBool = GetAnimationHash(settings.falling);
            
            lightAttackTrigger = GetAnimationHash(settings.lightAttack);
            heavyAttackTrigger = GetAnimationHash(settings.heavyAttack);
            cancelAttackTrigger = GetAnimationHash(settings.cancelAttack);
            jumpTrigger = GetAnimationHash(settings.jumping);
            dashTrigger = GetAnimationHash(settings.dashing);
            
            character.State.RegisterStateObserver(CharacterStates.IsFalling, OnFallStateChanged);
            character.State.RegisterStateObserver(CharacterStates.IsJumping, OnJumpStateChanged);
        }

        public void GroundHitLight()
        {
            PlayAnimation(groundHitLightAnim);
        }

        public void GroundHitHeavy()
        {
            PlayAnimation(groundHitHeavyAnim);
        }

        public void MoveLeft()
        {
            var scale = model.localScale;
            scale.x = -Mathf.Abs(scale.x);
            model.localScale = scale;
            SetBool(runBool, true);
        }

        public void MoveRight()
        {
            var scale = model.localScale;
            scale.x = Mathf.Abs(scale.x);
            model.localScale = scale;
            SetBool(runBool, true);
        }

        private void OnJumpStateChanged(bool isJumping)
        {
            if (!isJumping) return;
            SetTrigger(jumpTrigger);
        }

        private void OnFallStateChanged(bool isFalling)
        {
            SetBool(fallBool, isFalling);
        }

        public void Dash()
        {
            SetTrigger(dashTrigger);
        }

        public void StopMoving()
        {
            SetBool(runBool, false);
            SetBool(walkBool, false);
        }

        public void LightAttack(Action actionAttackCallback)
        {
            SetTrigger(lightAttackTrigger);
            animEvents.Attacked = actionAttackCallback;
        }

        public void HeavyAttack(Action actionAttackCallback)
        {
            SetTrigger(heavyAttackTrigger);
            animEvents.Attacked = actionAttackCallback;
        }

        public void CancelAttack()
        {
            SetTrigger(cancelAttackTrigger);
        }
        
        private static int GetAnimationHash(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return 0;
            return Animator.StringToHash(name);
        }

        private void PlayAnimation(int hash, int layer = 0)
        {
            if (hash == 0) return;
            animator.Play(hash, layer, 0f);
        }

        private void SetBool(int hash, bool value)
        {
            if (hash == 0) return;
            animator.SetBool(hash, value);
        }

        private void SetTrigger(int hash)
        {
            if (hash == 0) return;
            animator.SetTrigger(hash);
        }
    }
}