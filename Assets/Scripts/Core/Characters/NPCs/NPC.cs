using System;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest
{
    public enum CharacterID
    {
        None = 0,
        TestNPC = 1000,
    }
    
    public class NPC : MonoBehaviour, IInteractive
    {
        public CharacterProfile profile;
        
        public event Action<Character, CharacterProfile> OnInteractedWith = delegate { };

        public void OnInteract(Character character)
        {
            OnInteractedWith?.Invoke(character, profile);
        }

        public void OnOverlap(Character character)
        {
        }

        public void OnOverlapEnd(Character character)
        {
        }
    }
}