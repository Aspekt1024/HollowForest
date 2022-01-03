using System;
using System.Linq;
using HollowForest.Combat;
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
            public string dead;
            public string walking;
            public string running;
            public string falling;
            public string charging;
            
            [Header("Animation Triggers")]
            public string lightAttack;
            public string lightAttackCombo;
            public string heavyAttack;
            public string cancelAttack;
            public string dashing;
            public string jumping;
        }

        private readonly Animator animator;
        private readonly Transform model;
        private readonly CombatAnimationHandler combatAnim;

        private readonly int groundHitLightAnim;
        private readonly int groundHitHeavyAnim;
        private readonly int lightAttackTrigger;
        private readonly int lightComboTrigger;
        private readonly int heavyAttackTrigger;
        private readonly int cancelAttackTrigger;
        
        private readonly int dashTrigger;
        private readonly int jumpTrigger;

        private readonly int deadBool;
        private readonly int walkBool;
        private readonly int runBool;
        private readonly int fallBool;
        private readonly int chargeBool;

        public CharacterAnimator(Character character, Settings settings, Transform model)
        {
            animator = settings.animator;
            this.model = model;
            combatAnim = animator.GetComponentInChildren<CombatAnimationHandler>();

            groundHitLightAnim = GetAnimationHash(settings.groundHitLight);
            groundHitHeavyAnim = GetAnimationHash(settings.groundHitHeavy);

            deadBool = GetAnimationHash(settings.dead);
            walkBool = GetAnimationHash(settings.walking);
            runBool = GetAnimationHash(settings.running);
            fallBool = GetAnimationHash(settings.falling);
            chargeBool = GetAnimationHash(settings.charging);
            
            lightAttackTrigger = GetAnimationHash(settings.lightAttack);
            lightComboTrigger = GetAnimationHash(settings.lightAttackCombo);
            heavyAttackTrigger = GetAnimationHash(settings.heavyAttack);
            cancelAttackTrigger = GetAnimationHash(settings.cancelAttack);
            jumpTrigger = GetAnimationHash(settings.jumping);
            dashTrigger = GetAnimationHash(settings.dashing);

            if (character != null)
            {
                character.State.RegisterStateObserver(CharacterStates.IsFalling, OnFallStateChanged);
                character.State.RegisterStateObserver(CharacterStates.IsJumping, OnJumpStateChanged);
                character.State.RegisterStateObserver(CharacterStates.IsAlive, OnAliveStateChanged);
            }
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
            LookLeft();
            SetBool(runBool, true);
        }

        public void MoveRight()
        {
            LookRight();
            SetBool(runBool, true);
        }

        public void LookLeft()
        {
            var scale = model.localScale;
            scale.x = -Mathf.Abs(scale.x);
            model.localScale = scale;
        }

        public void LookRight()
        {
            var scale = model.localScale;
            scale.x = Mathf.Abs(scale.x);
            model.localScale = scale;
        }

        private void OnAliveStateChanged(bool isAlive)
        {
            SetBool(deadBool, !isAlive);
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

        public void SetCharging(bool isCharging)
        {
            SetBool(chargeBool, isCharging);
        }

        public void StopMoving()
        {
            SetBool(runBool, false);
            SetBool(walkBool, false);
        }

        public void LightAttack(Action actionAttackCallback)
        {
            if (combatAnim.IsComboAvailable)
            {
                SetTrigger(lightComboTrigger);
                combatAnim.OnAttack = actionAttackCallback;
            }
            else
            {
                ClearTrigger(lightComboTrigger);
                SetTrigger(lightAttackTrigger);
                combatAnim.OnAttack = actionAttackCallback;
            }
        }

        public void HeavyAttack(Action actionAttackCallback)
        {
            SetTrigger(heavyAttackTrigger);
            combatAnim.OnAttack = actionAttackCallback;
        }

        public void CancelAttack()
        {
            SetTrigger(cancelAttackTrigger);
        }
        
        private int GetAnimationHash(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return 0;
            var hash = Animator.StringToHash(name);
            if (animator.parameters.Any(p => p.nameHash == hash))
            {
                return hash;
            }
            
            Debug.LogWarning($"{animator.GetComponentInParent<Character>().name} has no animation parameter: {name}");
            return 0;
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

        private void ClearTrigger(int hash)
        {
            if (hash == 0) return;
            animator.ResetTrigger(hash);
        }
    }
}