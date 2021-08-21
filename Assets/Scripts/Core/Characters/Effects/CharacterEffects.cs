using System;
using UnityEngine;

namespace HollowForest.Effects
{
    public class CharacterEffects
    {
        [Serializable]
        public class Settings
        {
            public Animator animator;
            public Transform model;
            public ParticleSystem landedEffect;
        }

        private readonly Settings settings;
        private readonly Rigidbody2D body;

        private static readonly int SmallSquishAnim = Animator.StringToHash("SmallSquish");
        private static readonly int BigSquishAnim = Animator.StringToHash("BigSquish");

        public CharacterEffects(Character character, Settings settings)
        {
            this.settings = settings;
            body = character.Rigidbody;

            character.State.RegisterStateObserver(CharacterStates.Grounded, OnGroundedStateChanged);
        }

        private void OnGroundedStateChanged(bool isGrounded)
        {
            if (isGrounded)
            {
                OnGrounded();
            }
        }

        private void OnGrounded()
        {
            settings.model.rotation = Quaternion.identity;
            body.angularVelocity = 0f;
            
            settings.landedEffect.Play();
        }

        public void FallMinor()
        {
            settings.animator.Play(SmallSquishAnim, 0, 0f);
        }

        public void FallMajor()
        {
            settings.animator.Play(BigSquishAnim, 0, 0f);
        }
    }
}