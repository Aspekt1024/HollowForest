using System;
using HollowForest.Combat;
using UnityEngine;

namespace HollowForest
{
    public class CharacterCombat
    {
        [Serializable]
        public class Settings
        {
            public float heavyAttackLockTime = 0.6f;

            public int lightAttackBaseDamage = 1;
            public int heavyAttackBaseDamage = 2;

            public bool lockPositionForLightAttack = true;
        }

        [Serializable]
        public class CollisionSettings
        {
            public AttackCollider lightAttackCollider;
            public AttackCollider heavyAttackCollider;
        }
        
        private readonly Character character;
        private readonly Settings settings;
        private readonly CollisionSettings collisions;

        private readonly CombatAnimationHandler combatAnim;

        private float timeAttackLockEnds;
        private bool isLockedForAttack;
        
        public CharacterCombat(Character character, Settings settings, CollisionSettings collisions)
        {
            this.character = character;
            this.settings = settings;
            this.collisions = collisions;
            combatAnim = character.GetComponentInChildren<CombatAnimationHandler>();
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
            if (!combatAnim.CanAttack) return;
            character.State.SetState(CharacterStates.IsLockedForAttack, settings.lockPositionForLightAttack);
            character.Animator.LightAttack(() =>
            {
                character.State.SetState(CharacterStates.IsLockedForAttack, false); // TODO setup completion callback
                collisions.lightAttackCollider.ActionAttack((c, dir) =>
                {
                    var details = new HitDetails
                    {
                        source = character,
                        damage = settings.lightAttackBaseDamage,
                        direction = dir,
                    };
                    c.TakeDamage(details);
                });
            });
        }

        public void AttackLightReleased()
        {
            
        }
        
        public void AttackHeavyRequested()
        {
            if (!combatAnim.CanAttack) return;
            isLockedForAttack = true;
            timeAttackLockEnds = Time.time + settings.heavyAttackLockTime;
            character.State.SetState(CharacterStates.IsLockedForAttack, true);
            character.Animator.HeavyAttack(() =>
            {
                collisions.heavyAttackCollider.ActionAttack((c, dir) =>
                {
                    var details = new HitDetails
                    {
                        source = character,
                        damage = settings.heavyAttackBaseDamage,
                        direction = dir,
                    };
                    c.TakeDamage(details);
                });
            });
        }

        public void AttackHeavyReleased()
        {
            
        }

        public void BlockInput()
        {
            // TODO if holding an attack, cancel it with no effect
            character.Animator.CancelAttack();
            isLockedForAttack = false;
            character.State.SetState(CharacterStates.IsLockedForAttack, false);
        }
    }
}