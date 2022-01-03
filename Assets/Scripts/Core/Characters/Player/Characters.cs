using System.Collections.Generic;
using HollowForest.UI;

namespace HollowForest
{
    public class Characters
    {
        private readonly Game game;

        private Character playerCharacter;
        private readonly List<NPC> npcs = new List<NPC>();

        public bool IsPlayer(Character character) => playerCharacter == character;

        public Characters(Game game)
        {
            this.game = game;
        }

        public Character GetPlayerCharacter() => playerCharacter;
        
        public void RegisterPlayerCharacter(Character character)
        {
            if (playerCharacter != null)
            {
                UnregisterPlayerCharacter(playerCharacter);
            }
            playerCharacter = character;
            Game.UI.GetUI<HUD>().SetupCharacter(character);
        }

        public void UnregisterPlayerCharacter(Character character)
        {
            // TODO hud
            playerCharacter = null;
        }

        public void RegisterNPC(NPC npc)
        {
            npcs.Add(npc);
            npc.OnInteractedWith += OnInteractedWith;
        }

        public void UnregisterNPCs()
        {
            foreach (var npc in npcs)
            {
                npc.OnInteractedWith -= OnInteractedWith;
            }
            npcs.Clear();
        }

        public void Tick_Fixed()
        {
            if (playerCharacter != null)
            {
                playerCharacter.Physics.Tick_Fixed();
            }
        }

        public void BlockInput(float duration)
        {
            if (playerCharacter != null)
            {
                playerCharacter.Director.BlockInputs(duration);
            }
        }

        private void OnInteractedWith(Character interactingCharacter, CharacterRef characterInteractedWith)
        {
            game.dialogue.InitiateDialogue(interactingCharacter, characterInteractedWith, null);
        }
    }
}