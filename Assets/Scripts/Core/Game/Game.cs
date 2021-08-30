using HollowForest.Data;
using HollowForest.Events;
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
        public Configuration configuration;

        [HideInInspector] public Dialogue.Dialogue dialogue;
        [HideInInspector] public Characters characters;
        [HideInInspector] public Objects.Objects objects;
        [HideInInspector] public Data.Data data;
        [HideInInspector] public GameplayEvents events;

        private void Awake()
        {
            characters = new Characters(this);
            objects = new Objects.Objects(this);
            dialogue = new Dialogue.Dialogue();
            data = new Data.Data(configuration);
            events = new GameplayEvents(data);
            
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