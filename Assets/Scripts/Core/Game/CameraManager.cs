using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HollowForest
{
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera;
        public float followSpeed;
        public AnimationCurve squishCurve;
        public float baseShakeIntensity = 0.1f;

        private Transform mainCamTf;
        private Transform followTarget;
        
        private Transform tempTarget;
        private float tempTargetFollowEndTime;

        private float squishIntensity = 1f;
        private float squishStartTime;
        private float squishEndTime;
        private float originalOrthSize;
        
        private float shakeIntensity = 1f;
        private float shakeEndTime;

        private void Awake()
        {
            mainCamTf = mainCamera.transform;
            originalOrthSize = mainCamera.orthographicSize;
        }

        public void Follow(Transform target)
        {
            followTarget = target;
        }

        public void FollowTemporary(Transform target, float duration)
        {
            tempTarget = target;
            tempTargetFollowEndTime = Time.time + duration;
        }

        public void Squish(float intensity, float duration = 0.2f)
        {
            squishIntensity = intensity;
            squishStartTime = Time.time;
            squishEndTime = Time.time + duration;
        }

        public void Shake(float intensity, float duration = 0.5f)
        {
            shakeIntensity = intensity;
            shakeEndTime = Time.time + duration;
        }

        private void LateUpdate()
        {
            var target = Time.time >= tempTargetFollowEndTime ? followTarget : tempTarget;
            
            var pos = target.position;
            pos.z = mainCamTf.position.z;
            pos = Vector3.Lerp(mainCamTf.position, pos, Time.deltaTime * followSpeed);

            if (Time.time < squishEndTime)
            {
                var squishTimeRatio = (Time.time - squishStartTime) / (squishEndTime - squishStartTime);
                mainCamera.orthographicSize = originalOrthSize * squishIntensity * squishCurve.Evaluate(squishTimeRatio);
            }

            if (Time.time < shakeEndTime)
            {
                pos += (Vector3) Random.insideUnitCircle * (baseShakeIntensity * shakeIntensity);
            }
            
            mainCamTf.position = pos;
        }
    }
}