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

        public Dialogue.Dialogue dialogue;
        public Characters characters;
        public Objects.Objects objects;
        public Data.Data data;
        public GameplayEvents events;

        private static Game instance;

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
            dialogue.InitAwake(data.Config.dialogue, data.GameData.dialogue, ui);

            events.RegisterObserver(characters);
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

        public static Game GetInstance()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Game>();
            }
            return instance;
        }
    }
}