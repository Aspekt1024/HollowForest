using System;
using System.Threading.Tasks;
using HollowForest.Cam;
using HollowForest.Combat;
using UnityEngine;

namespace HollowForest.Effects
{
    public class DamageEffect : MonoBehaviour, Health.IDamageObserver
    {
        public ParticleSystem effect;

        [Header("Time dilation")]
        public bool hasTimeDilation = true;
        public float timeScale = 0.8f;
        public float dilationDuration = 0.2f;
        
        [Header("Screen Shake")]
        public bool hasScreenShake = true;
        public CameraShake.Intensity shakeIntensity = CameraShake.Intensity.Light;
        public float shakeDuration = 0.1f;

        [Header("Physics")]
        public bool canKnockback = true;
        public float knockbackDuration = 0.05f;
        public float verticalKnockbackFactor = 1f;
        public float horizontalKnockbackFactor = 20f;

        private Character character;

        public void Start()
        {
            character = GetComponentInParent<Character>();
            if (character == null)
            {
                Debug.LogWarning($"{nameof(DamageEffect)} must be a child of a {nameof(Character)}.");
                return;
            }

            character.Health.RegisterObserver(this);
        }

        public void OnDamageTaken(HitDetails details)
        {
            if (hasScreenShake)
            {
                Game.Camera.Shake(shakeIntensity, shakeDuration);   
            }

            if (hasTimeDilation)
            {
                SlowTime(timeScale, dilationDuration);
            }

            if (effect != null)
            {
                effect.Play();
            }

            if (canKnockback)
            {
                var knockbackVelocity = new Vector3(
                    Mathf.Sign(details.direction.x) * horizontalKnockbackFactor,
                    Mathf.Sign(details.direction.y) * verticalKnockbackFactor,
                    0f);
                
                character.Physics.OverrideVelocity(knockbackVelocity, knockbackDuration);
                character.Director.BlockInputs(knockbackDuration);
            }
        }

        private static async void SlowTime(float timeScale, float duration)
        {
            Time.timeScale = timeScale;
            await Task.Delay((int)(duration * 1000));
            Time.timeScale = 1f;
        }
    }
}