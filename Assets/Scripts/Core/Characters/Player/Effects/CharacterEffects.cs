using System;
using HollowForest.Cam;
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
            public bool hasLandedEffect = true;
        }

        private readonly Character character;
        private readonly Settings settings;
        private readonly Rigidbody2D body;

        public CharacterEffects(Character character, Settings settings)
        {
            this.character = character;
            this.settings = settings;
            body = character.Rigidbody;

            if (settings.dashEffect != null)
            {
                settings.dashEffect.emitting = false;
            }

            character.Physics.OnGoundHit += OnGroundHit;
        }

        private void OnGroundHit(Vector3 hitPos, float fallHeight)
        {
            if (!settings.hasLandedEffect) return;
            
            if (fallHeight > 2f)
            {
                settings.model.rotation = Quaternion.identity;
                body.angularVelocity = 0f;
                if (settings.landedEffect != null)
                {
                    settings.landedEffect.transform.position = hitPos;
                    settings.landedEffect.Play();
                }
            }
            
            if (fallHeight > 3f)
            {
                if (fallHeight > 6f)
                {
                    character.Animator.GroundHitHeavy();
                    Game.Camera.Shake(CameraShake.Intensity.Medium, 0.2f);
                    character.Afflictions.BeginFallRecovery();
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

            if (settings.wallAttachEffect != null)
            {
                settings.wallAttachEffect.transform.position = hitPosition;
                settings.wallAttachEffect.Play();
            }
        }

        public void BeginDash()
        {
            settings.dashEffect.emitting = true;
            character.Animator.Dash();
        }

        public void EndDash()
        {
            if (settings.dashEffect != null)
            {
                settings.dashEffect.emitting = false;
            }
        }
    }
}