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
        
        private readonly List<TrackedCharacter> trackedCharacters = new List<TrackedCharacter>();
        
        public void RegisterCharacter(Character character)
        {
            trackedCharacters.Add(new TrackedCharacter(character));
            character.Physics.OnGoundHit += OnGoundHit;
            character.Physics.OnAttachedToWall += OnAttachedToWall;
        }

        public void UnregisterCharacter(Character character)
        {
            var index = trackedCharacters.FindIndex(c => c.Character == character);
            if (index < 0) return;
            
            trackedCharacters[index].Character.Physics.OnGoundHit -= OnGoundHit;
            trackedCharacters[index].Character.Physics.OnAttachedToWall -= OnAttachedToWall;
            trackedCharacters.RemoveAt(index);
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

        private void OnAttachedToWall(Character character, Vector3 attachPos)
        {
            character.Effects.OnAttachedToWall(attachPos);
        }
    }
}