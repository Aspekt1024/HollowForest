using System;
using UnityEngine;

namespace HollowForest.Cam
{
    public class CameraSquish
    {
        [Serializable]
        public class Settings
        {
            public AnimationCurve squishCurve;
        }

        private readonly Settings settings;
        private readonly Camera camera;

        public CameraSquish(Settings settings, Camera camera)
        {
            this.settings = settings;
            this.camera = camera;
            originalOrthSize = camera.orthographicSize;
        }
        
        private float squishIntensity = 1f;
        private float squishStartTime;
        private float squishEndTime;
        private float originalOrthSize;

        public void Begin(float intensity, float duration)
        {
            squishIntensity = intensity;
            squishStartTime = Time.time;
            squishEndTime = Time.time + duration;
        }

        public void Tick_Late()
        {
            if (Time.time < squishEndTime)
            {
                var squishTimeRatio = (Time.time - squishStartTime) / (squishEndTime - squishStartTime);
                camera.orthographicSize = originalOrthSize * squishIntensity * settings.squishCurve.Evaluate(squishTimeRatio);
            }
        }
    }
}