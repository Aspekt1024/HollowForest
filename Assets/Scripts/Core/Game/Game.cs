using HollowForest.UI;
using UnityEngine;

namespace HollowForest
{
    public class Game : MonoBehaviour
    {
        public InputManager inputManager;
        public CameraManager cameraManager;
        public UserInterface ui;

        public Character player;

        [HideInInspector] public Dialogue.Dialogue dialogue;
        [HideInInspector] public Characters characters;

        private void Awake()
        {
            characters = new Characters(this);
            dialogue = new Dialogue.Dialogue();
            
            Initialise();
        }

        private void Initialise()
        {
            // Order of initialisation is critical here to avoid race conditions
            ui.InitAwake();
            inputManager.InitAwake(ui);
            dialogue.InitAwake(ui);
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