using System;
using UnityEngine;

namespace HollowForest.World
{
    public class Background : MonoBehaviour
    {
        private Transform mainCamTf;

        private void Awake()
        {
            var camManager = FindObjectOfType<CameraManager>();
            mainCamTf = camManager.mainCamera.transform;
        }

        private void LateUpdate()
        {
            var pos = transform.position;
            var camPos = mainCamTf.position;

            pos.x = camPos.x - (camPos.x % 1f) + 0.5f;
            pos.y = camPos.y - (camPos.y % 1f) + 0.5f;
            
            transform.position = pos;
        }
    }
}