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
        }

        public void OnGroundHit(Vector3 hitPosition, float fallHeight)
        {
            if (fallHeight > 2f)
            {
                settings.model.rotation = Quaternion.identity;
                body.angularVelocity = 0f;
                settings.landedEffect.transform.position = hitPosition;
                settings.landedEffect.Play();
            }
            
            if (fallHeight > 3f)
            {
                if (fallHeight > 6f)
                {
                    settings.animator.Play(BigSquishAnim, 0, 0f);
                }
                else
                {
                    settings.animator.Play(SmallSquishAnim, 0, 0f);
                }
            }
        }

        public void OnAttachedToWall(Vector3 hitPosition)
        {
            
        }
    }
}