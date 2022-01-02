using System.Collections.Generic;
using System.Linq;
using HollowForest.UI;
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

        public bool IsPlayer(Character character) => trackedCharacters.Any(c => c.Character == character);

        public Characters(Game game)
        {
            this.game = game;
        }

        public Character GetPlayerCharacter()
        {
            return trackedCharacters[0].Character;
        }
        
        public void RegisterCharacter(Character character)
        {
            trackedCharacters.Add(new TrackedCharacter(character));
            character.Physics.OnGoundHit += OnGoundHit;
            Game.UI.GetUI<HUD>().SetupCharacter(character);
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
            foreach (var character in trackedCharacters)
            {
                character.Character.Physics.Tick_Fixed();
            }
        }

        public void BlockInput(float duration)
        {
            trackedCharacters.ForEach(c => c.Character.Director.BlockInputs(duration));
        }

        private void OnGoundHit(Character character, Vector3 hitPos, float fallHeight)
        {
            // TODO setup ground landing particle effect in some manager
            character.Effects.OnGroundHit(hitPos, fallHeight);
        }

        private void OnInteractedWith(Character interactingCharacter, CharacterRef characterInteractedWith)
        {
            game.dialogue.InitiateDialogue(interactingCharacter, characterInteractedWith, null);
        }
    }
}