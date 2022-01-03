
using System;
using UnityEngine;

namespace HollowForest.Cam
{
    public class CameraBounds
    {
        [Serializable]
        public class Settings
        {
            public float boundPositionLerpDuration = 1.3f;
        }
        
        private bool isBound;
        private Vector3 boundsCentre;
        private Vector2 boundsSize;
        private float boundStartTime;
        private const float BoundPositionLerpDuration = 1.3f;

        private readonly Settings settings;
        private readonly Camera camera;

        public CameraBounds(Settings settings, Camera camera)
        {
            this.settings = settings;
            this.camera = camera;
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

        public Vector3 UpdateCameraPosition(Vector3 pos)
        {
            if (!isBound) return pos;
            
            var orthSize = camera.orthographicSize;
            var xExtent = boundsSize.x * 0.5f - camera.aspect * orthSize;
            var yExtent = boundsSize.y * 0.5f - orthSize;

            var minX = boundsCentre.x - xExtent;
            var maxX = boundsCentre.x + xExtent;
            var minY = boundsCentre.y - yExtent;
            var maxY = boundsCentre.y + yExtent;
            var xPos = minX < maxX ? Mathf.Clamp(pos.x, minX, maxX) : boundsCentre.x;
            var yPos = minY < maxY ? Mathf.Clamp(pos.y, minY, maxY) : boundsCentre.y;
            var newPos = new Vector2(xPos, yPos);

            var lerpRatio = (Time.time - boundStartTime) / BoundPositionLerpDuration;
            pos.x = Mathf.Lerp(pos.x, newPos.x, lerpRatio);
            pos.y = Mathf.Lerp(pos.y, newPos.y, lerpRatio);
            
            return pos;
        }
    }
}