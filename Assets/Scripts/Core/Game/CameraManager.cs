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

        private bool isBound;
        private Vector3 boundsCentre;
        private Vector2 boundsSize;
        private float boundStartTime;
        private const float BoundPositionLerpDuration = 5f;

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

        public void ApplyBounds(Vector3 centre, Vector2 size)
        {
            isBound = true;
            boundsCentre = centre;
            boundsSize = size + Vector2.one * 2f;
            boundStartTime = Time.time;
        }

        public void ReleaseBounds()
        {
            isBound = false;
        }

        private void LateUpdate()
        {
            var target = Time.time >= tempTargetFollowEndTime ? followTarget : tempTarget;
            
            var pos = target.position;
            pos.z = mainCamTf.position.z;

            if (isBound)
            {
                pos = HandleBoundPosition(pos);
            }
            
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

        private Vector3 HandleBoundPosition(Vector3 pos)
        {
            var xExtent = boundsSize.x * 0.5f - mainCamera.aspect * mainCamera.orthographicSize;
            var yExtent = boundsSize.y * 0.5f - mainCamera.orthographicSize;

            var newPos = new Vector2(
                Mathf.Clamp(pos.x, boundsCentre.x - xExtent, boundsCentre.x + xExtent),
                Mathf.Clamp(pos.y, boundsCentre.y - yExtent, boundsCentre.y + yExtent)
            );

            var lerpRatio = (Time.time - boundStartTime) / BoundPositionLerpDuration;
            pos.x = Mathf.Lerp(pos.x, newPos.x, lerpRatio);
            pos.y = Mathf.Lerp(pos.y, newPos.y, lerpRatio);
            
            return pos;
        }
    }
}