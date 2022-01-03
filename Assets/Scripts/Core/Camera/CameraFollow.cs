using System;
using UnityEngine;

namespace HollowForest.Cam
{
    public class CameraFollow
    {
        [Serializable]
        public class Settings
        {
            public float followSpeed = 4.2f;
        }

        private readonly Settings settings;
        private readonly Transform cameraTf;

        private Transform followTarget;
        
        private Transform tempTarget;
        private float tempTargetFollowEndTime;
        
        public CameraFollow(Settings settings, Camera camera)
        {
            this.settings = settings;
            cameraTf = camera.transform;
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

        public Vector3 GetTargetPosition()
        {
            var target = Time.time >= tempTargetFollowEndTime ? followTarget : tempTarget;
            return target.position;
        }

        public Vector3 UpdateCameraPosition(Vector3 pos)
        {
            var currentPos = cameraTf.position;
            var diff = Vector3.Distance(currentPos, pos);
            var speedModifier = Time.time < tempTargetFollowEndTime ? 1f : Mathf.Max(1f, diff);
            return Vector3.Lerp(currentPos, pos, Time.deltaTime * settings.followSpeed * speedModifier);
        }
    }
}