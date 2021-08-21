using UnityEngine;

namespace HollowForest
{
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera;
        public float followSpeed;
        
        private Transform target;
        
        public void Follow(Transform target)
        {
            this.target = target;
        }

        private void LateUpdate()
        {
            var pos = target.position;
            pos.z = mainCamera.transform.position.z;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, pos, Time.deltaTime * followSpeed);
        }
    }
}