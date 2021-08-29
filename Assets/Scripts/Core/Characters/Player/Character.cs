using HollowForest.Effects;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest
{
    public class Character : MonoBehaviour
    {
        public CharacterSettings settings;
        public CharacterEffects.Settings effectsSettings;
        
        public CharacterState State { get; private set; }
        public CharacterDirector Director { get; private set; }
        public CharacterPhysics Physics { get; private set; }
        public CharacterAfflictions Afflictions { get; private set; }
        public CharacterEffects Effects { get; private set; }
        public Interaction Interaction { get; private set; }
        
        public Transform Transform { get; private set; }
        public Collider2D Collider { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }

        private void Awake()
        {
            Transform = transform;
            Collider = GetComponent<Collider2D>();
            Rigidbody = GetComponent<Rigidbody2D>();
            
            State = new CharacterState();
            Physics = new CharacterPhysics(this, settings.physicsSettings);
            Director = new CharacterDirector(this, Physics);
            Afflictions = new CharacterAfflictions(this, settings.afflictionSettings);
            Effects = new CharacterEffects(this, effectsSettings);
            Interaction = new Interaction(this);
        }

        private void Update()
        {
            Afflictions.Tick();
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
    }
}