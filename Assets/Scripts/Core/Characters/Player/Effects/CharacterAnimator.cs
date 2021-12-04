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
            public string lightAttack;
            public string heavyAttack;

            [Header("Animation Booleans")]
            public string walking;
            public string running;
        }

        private readonly Animator animator;
        private readonly Transform model;

        private readonly int groundHitLightAnim;
        private readonly int groundHitHeavyAnim;
        private readonly int lightAttackAnim;
        private readonly int heavyAttackAnim;
        
        private readonly int walkBool;
        private readonly int runBool;

        public CharacterAnimator(Settings settings, Transform model)
        {
            animator = settings.animator;
            this.model = model;

            groundHitLightAnim = GetAnimationHash(settings.groundHitLight);
            groundHitHeavyAnim = GetAnimationHash(settings.groundHitHeavy);
            lightAttackAnim = GetAnimationHash(settings.lightAttack);
            heavyAttackAnim = GetAnimationHash(settings.heavyAttack);

            walkBool = GetAnimationHash(settings.walking);
            runBool = GetAnimationHash(settings.running);
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
            model.localEulerAngles = new Vector3(0f, 180f, 0f);
            SetBool(runBool, true);
        }

        public void MoveRight()
        {
            model.localEulerAngles = Vector3.zero;
            SetBool(runBool, true);
        }

        public void StopMoving()
        {
            SetBool(runBool, false);
            SetBool(walkBool, false);
        }

        public void LightAttack()
        {
            PlayAnimation(lightAttackAnim);
        }

        public void HeavyAttack()
        {
            PlayAnimation(heavyAttackAnim);
        }
        
        private static int GetAnimationHash(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return 0;
            return Animator.StringToHash(name);
        }

        private void PlayAnimation(int hash)
        {
            if (hash == 0) return;
            animator.Play(hash, 0, 0f);
        }

        private void SetBool(int hash, bool value)
        {
            if (hash == 0) return;
            animator.SetBool(hash, value);
        }
    }
}