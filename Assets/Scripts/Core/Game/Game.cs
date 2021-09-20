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

        public static CameraManager Camera => Instance.cameraManager;
        public static Dialogue.Dialogue Dialogue => Instance.dialogue;
        public static Characters Characters => Instance.characters;
        public static Objects.Objects Objects => Instance.objects;
        public static Data.Data Data => Instance.data;
        public static GameplayEvents Events => Instance.events;

        private static Game instance;

        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Game>();
                }
                return instance;
            }
        }

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
    }
}