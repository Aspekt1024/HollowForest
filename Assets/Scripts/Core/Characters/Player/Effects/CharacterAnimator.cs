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

            [Header("Animation Triggers")]
            public string lightAttack;
            public string heavyAttack;
        }

        private readonly Animator animator;
        private readonly Transform model;

        private readonly int groundHitLightAnim;
        private readonly int groundHitHeavyAnim;
        private readonly int lightAttackTrigger;
        private readonly int heavyAttackTrigger;
        
        private readonly int walkBool;
        private readonly int runBool;

        public CharacterAnimator(Settings settings, Transform model)
        {
            animator = settings.animator;
            this.model = model;

            groundHitLightAnim = GetAnimationHash(settings.groundHitLight);
            groundHitHeavyAnim = GetAnimationHash(settings.groundHitHeavy);

            walkBool = GetAnimationHash(settings.walking);
            runBool = GetAnimationHash(settings.running);
            
            lightAttackTrigger = GetAnimationHash(settings.lightAttack);
            heavyAttackTrigger = GetAnimationHash(settings.heavyAttack);
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

        public void StopMoving()
        {
            SetBool(runBool, false);
            SetBool(walkBool, false);
        }

        public void LightAttack()
        {
            SetTrigger(lightAttackTrigger);
        }

        public void HeavyAttack()
        {
            SetTrigger(heavyAttackTrigger);
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