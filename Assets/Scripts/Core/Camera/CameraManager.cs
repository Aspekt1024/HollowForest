using HollowForest.UI;
using UnityEngine;

namespace HollowForest.Cam
{
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera;
        public CameraShake.Settings shakeSettings;
        public CameraSquish.Settings squishSettings;
        public CameraBounds.Settings boundsSettings;
        public CameraFollow.Settings followSettings;

        private UserInterface ui;

        private Transform mainCamTf;

        private CameraShake shake;
        private CameraSquish squish;
        private CameraBounds bounds;
        private CameraFollow follow;

        private void Awake()
        {
            mainCamTf = mainCamera.transform;

            shake = new CameraShake(shakeSettings);
            squish = new CameraSquish(squishSettings, mainCamera);
            bounds = new CameraBounds(boundsSettings, mainCamera);
            follow = new CameraFollow(followSettings, mainCamera);
        }

        public void InitAwake(UserInterface ui)
        {
            this.ui = ui;
        }

        public void Follow(Transform target) => follow.Follow(target);

        public void FollowTemporary(Transform target, float duration, bool blockInput)
        {
            follow.FollowTemporary(target, duration);
            if (blockInput)
            {
                SetCinematic(duration);
            }
        }

        public void SetCinematic(float duration)
        {
            ui.GetUI<CinematicUI>().Activate(duration);
            Game.Characters.BlockInput(duration);
        }

        public void Squish(float intensity, float duration = 0.2f) => squish.Begin(intensity, duration);

        public void Shake(CameraShake.Intensity intensity, float duration = 0.5f) => shake.Begin(intensity, duration);

        public void ApplyBounds(Vector3 centre, Vector2 size) => bounds.ApplyBounds(centre, size);
        public void ReleaseBounds() => bounds.ReleaseBounds();

        private void LateUpdate()
        {
            squish.Tick_Late();
            
            var currentPos = mainCamTf.position;
            var pos = follow.GetTargetPosition();
            pos.z = currentPos.z;

            pos = bounds.UpdateCameraPosition(pos);
            pos = follow.UpdateCameraPosition(pos);
            pos = shake.UpdateCameraPosition(pos);
            
            mainCamTf.position = pos;

        }
    }
}