using System;
using System.Collections.Generic;
using HollowForest.Combat;
using UnityEngine;

namespace HollowForest
{
    public class Health
    {
        [Serializable]
        public class Settings
        {
            public int baseHealth = 3;
            public float mitigation = 0f;
            public float invincibilityDurationAfterHit = 0f;
        }

        private readonly Character character;
        private readonly Settings settings;

        private int currentHealth;
        public event Action<int> HealthModified = delegate { };
        
        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                currentHealth = value;
                HealthModified?.Invoke(currentHealth);
            }
        }
        
        private int MaxHealth => settings.baseHealth;

        public interface IDamageObserver
        {
            void OnDamageTaken(HitDetails details);
        }

        private readonly List<IDamageObserver> damageObservers = new List<IDamageObserver>();

        public void RegisterObserver(IDamageObserver observer) => damageObservers.Add(observer);
        public void UnregisterObserver(IDamageObserver observer) => damageObservers.Remove(observer);
        
        public Health(Character character, Settings settings)
        {
            this.character = character;
            this.settings = settings;

            CurrentHealth = MaxHealth;
            character.State.SetState(CharacterStates.IsAlive, true);
        }

        public void TakeDamage(HitDetails details)
        {
            if (CurrentHealth <= 0) return;
            
            CurrentHealth = Mathf.Min(CurrentHealth - details.damage, MaxHealth);
            damageObservers.ForEach(o => o.OnDamageTaken(details));
            if (CurrentHealth <= 0)
            {
                character.State.SetState(CharacterStates.IsAlive, false);
            }
        }

        public void Heal(int health)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + health, MaxHealth);
        }
    }
}