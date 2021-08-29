using System.Collections.Generic;
using UnityEngine;

namespace HollowForest
{
    public class Characters
    {
        private readonly struct TrackedCharacter
        {
            public readonly Character Character;

            public TrackedCharacter(Character character)
            {
                Character = character;
            }
        }

        private readonly Game game;
        
        private readonly List<TrackedCharacter> trackedCharacters = new List<TrackedCharacter>();
        private readonly List<NPC> npcs = new List<NPC>();

        public Characters(Game game)
        {
            this.game = game;
            
            var npcsInScene = Object.FindObjectsOfType<NPC>();
            foreach (var npc in npcsInScene)
            {
                RegisterNPC(npc);
            }
        }
        
        public void RegisterCharacter(Character character)
        {
            trackedCharacters.Add(new TrackedCharacter(character));
            character.Physics.OnGoundHit += OnGoundHit;
        }

        public void UnregisterCharacter(Character character)
        {
            var index = trackedCharacters.FindIndex(c => c.Character == character);
            if (index < 0) return;
            
            trackedCharacters[index].Character.Physics.OnGoundHit -= OnGoundHit;
            trackedCharacters.RemoveAt(index);
        }

        public void RegisterNPC(NPC npc)
        {
            npcs.Add(npc);
            npc.OnInteractedWith += OnInteractedWith;
        }

        public void UnregisterNPC(NPC npc)
        {
            npcs.Remove(npc);
            npc.OnInteractedWith -= OnInteractedWith;
        }

        public void Tick_Fixed()
        {
            foreach (var character in trackedCharacters)
            {
                character.Character.Physics.Tick_Fixed();
            }
        }

        private void OnGoundHit(Character character, Vector3 hitPos, float fallHeight)
        {
            // TODO setup ground landing particle effect in some manager
            character.Effects.OnGroundHit(hitPos, fallHeight);
        }

        private void OnInteractedWith(Character interactingCharacter, CharacterID characterIDInteractedWith)
        {
            // TODO determine dialogue for given character-to-character interaction
            var testDialogue = new List<string>()
            {
                characterIDInteractedWith + " says hello.",
                "Goodbye now."
            };
            game.dialogue.BeginDialogue(testDialogue);
        }
    }
}