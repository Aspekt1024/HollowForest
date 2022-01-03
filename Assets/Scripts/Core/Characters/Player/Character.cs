using System.Collections.Generic;
using HollowForest.Combat;
using HollowForest.Effects;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest
{
    public class Character : MonoBehaviour
    {
        public CharacterSettings settings;
        public CharacterAnimator.Settings animatorSettings;
        public CharacterEffects.Settings effectsSettings;
        public CharacterCombat.CollisionSettings combatCollisionSettings;
        
        public CharacterState State { get; private set; }
        public Health Health { get; private set; }
        public CharacterAbilities Abilities { get; private set; }
        public CharacterDirector Director { get; private set; }
        public CharacterPhysics Physics { get; private set; }
        public CharacterAfflictions Afflictions { get; private set; }
        public CharacterEffects Effects { get; private set; }
        public CharacterAnimator Animator { get; private set; }
        public CharacterCombat Combat { get; private set; }
        public Interaction Interaction { get; private set; }
        
        public Transform Transform { get; private set; }
        public Collider2D Collider { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }

        public bool IsPlayer() => Game.Characters.IsPlayer(this);

        private void Awake()
        {
            Transform = transform;
            Collider = GetComponent<Collider2D>();
            Rigidbody = GetComponent<Rigidbody2D>();
            
            State = new CharacterState();
            Animator = new CharacterAnimator(this, animatorSettings, effectsSettings.model);
            
            Health = new Health(this, settings.healthSettings);
            Abilities = new CharacterAbilities();
            Physics = new CharacterPhysics(this, settings.physicsSettings);
            Combat = new CharacterCombat(this, settings.combatSettings, combatCollisionSettings);
            Director = new CharacterDirector(this, Physics, Combat);
            Afflictions = new CharacterAfflictions(this, settings.afflictionSettings);
            Effects = new CharacterEffects(this, effectsSettings);
            Interaction = new Interaction(this);
            
            SetBaseAbilities();
        }

        protected virtual void Update()
        {
            Director.Tick();
            Afflictions.Tick();
            Combat.Tick();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Layers.IsLayerMatch(other.gameObject.layer, Layers.Interactive))
            {
                Interaction.SetInteractive(other.GetComponent<IInteractive>());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (Layers.IsLayerMatch(other.gameObject.layer, Layers.Interactive))
            {
                Interaction.UnsetInteractive(other.GetComponent<IInteractive>());
            }
        }

        public void TakeDamage(HitDetails hitDetails)
        {
            Health.TakeDamage(hitDetails);
        }

        private void SetBaseAbilities()
        {
            Abilities.EnableAbility(CharacterAbility.Jump);
        }
    }
}