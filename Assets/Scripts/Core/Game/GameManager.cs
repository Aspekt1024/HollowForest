using UnityEngine;

namespace HollowForest
{
    public class GameManager : MonoBehaviour
    {
        public InputManager inputManager;
        public CameraManager cameraManager;

        public Character player;

        private Characters characters;

        private void Awake()
        {
            characters = new Characters();
        }

        private void Start()
        {
            inputManager.SetCharacter(player);
            characters.RegisterCharacter(player);
            cameraManager.Follow(player.transform);
        }

        private void FixedUpdate()
        {
            characters.Tick_Fixed();
        }
    }
}