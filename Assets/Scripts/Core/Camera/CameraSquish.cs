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
        
        private float squishIntensity = 1f;
        private float squishStartTime;
        private float squishEndTime;
        
        private readonly float fieldOfView;

        public CameraSquish(Settings settings, Camera camera)
        {
            this.settings = settings;
            this.camera = camera;
            fieldOfView = camera.fieldOfView;
        }

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
                camera.fieldOfView = fieldOfView * squishIntensity * settings.squishCurve.Evaluate(squishTimeRatio);
            }
        }
    }
}