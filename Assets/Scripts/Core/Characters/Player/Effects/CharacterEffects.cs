using System;
using UnityEngine;

namespace HollowForest.Effects
{
    public class CharacterEffects
    {
        [Serializable]
        public class Settings
        {
            public Transform model;
            public ParticleSystem landedEffect;
            public ParticleSystem wallAttachEffect;
            public TrailRenderer dashEffect;
            public SpriteRenderer slashEffect;
        }

        private readonly Character character;
        private readonly Settings settings;
        private readonly Rigidbody2D body;

        public CharacterEffects(Character character, Settings settings)
        {
            this.character = character;
            this.settings = settings;
            body = character.Rigidbody;

            settings.dashEffect.emitting = false;
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
                    character.Animator.GroundHitHeavy();
                    Game.Camera.Shake(1f, 0.2f);
                }
                else
                {
                    character.Animator.GroundHitLight();
                    Game.Camera.Squish(1f);
                }
            }
        }

        public void OnAttachedToWall(Vector3 hitPosition)
        {
            settings.model.rotation = Quaternion.identity;
            body.angularVelocity = 0f;
            
            settings.wallAttachEffect.transform.position = hitPosition;
            settings.wallAttachEffect.Play();
        }

        public void BeginDash()
        {
            settings.dashEffect.emitting = true;
            character.Animator.Dash();
        }

        public void EndDash()
        {
            settings.dashEffect.emitting = false;
        }
    }
}