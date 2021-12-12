using System;
using UnityEngine;

namespace HollowForest
{
    public class CharacterCombat
    {
        [Serializable]
        public class Settings
        {
            public float lightAttackCooldown = 0.33f;
            public float heavyAttackCooldown = 0.95f;
            public float heavyAttackLockTime = 0.6f;
        }
        
        private readonly Character character;
        private readonly Settings settings;

        private float timeNextAttackAvailable;
        private float timeAttackLockEnds;
        private bool isLockedForAttack;
        
        public CharacterCombat(Character character, Settings settings)
        {
            this.character = character;
            this.settings = settings;

            timeNextAttackAvailable = -1000f;
        }

        public void Tick()
        {
            if (isLockedForAttack && Time.time >= timeAttackLockEnds)
            {
                isLockedForAttack = false;
                character.State.SetState(CharacterStates.IsLockedForAttack, false);
            }
        }
        
        public void AttackLightRequested()
        {
            if (!CanAttack()) return;
            timeNextAttackAvailable = Time.time + settings.lightAttackCooldown;
            character.Animator.LightAttack();
        }

        public void AttackLightReleased()
        {
            
        }
        
        public void AttackHeavyRequested()
        {
            if (!CanAttack()) return;
            isLockedForAttack = true;
            timeNextAttackAvailable = Time.time + settings.heavyAttackCooldown;
            timeAttackLockEnds = Time.time + settings.heavyAttackLockTime;
            character.State.SetState(CharacterStates.IsLockedForAttack, true);
            character.Animator.HeavyAttack();
        }

        public void AttackHeavyReleased()
        {
            
        }

        public void BlockInput()
        {
            // TODO if holding an attack, cancel it with no effect
        }

        private bool CanAttack()
        {
            return Time.time >= timeNextAttackAvailable;
        }
    }
}