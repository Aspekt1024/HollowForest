using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HollowForest.Cam
{
    public class CameraShake
    {
        public enum Intensity
        {
            None = 0,
            Light = 1000,
            Medium = 2000,
            Heavy = 3000,
            Mega = 4000,
        }
        
        [Serializable]
        public class Settings
        {
            public float baseShakeIntensity = 0.1f;
        }

        private static readonly Dictionary<Intensity, float> IntensityDict = new Dictionary<Intensity, float>
        {
            { Intensity.None, 0f },
            { Intensity.Light, 0.4f },
            { Intensity.Medium, 1f },
            { Intensity.Heavy, 1.9f },
            { Intensity.Mega, 3f },
        };

        private readonly Settings settings;
        
        private float shakeIntensity = 1f;
        private float shakeEndTime;

        public CameraShake(Settings settings)
        {
            this.settings = settings;
        }

        public void Begin(Intensity intensity, float duration)
        {
            shakeIntensity = EvaluateIntensity(intensity);
            shakeEndTime = Time.unscaledTime + duration;
            
        }

        public Vector3 UpdateCameraPosition(Vector3 pos)
        {
            if (Time.unscaledTime < shakeEndTime)
            {
                pos += (Vector3) Random.insideUnitCircle * (settings.baseShakeIntensity * shakeIntensity);
            }

            return pos;
        }

        private float EvaluateIntensity(Intensity intensity)
        {
            const Intensity intensityFallbackLevel = Intensity.Medium;
            
            return IntensityDict.ContainsKey(intensity)
                ? IntensityDict[intensity]
                : IntensityDict[intensityFallbackLevel];
        }
    }
}