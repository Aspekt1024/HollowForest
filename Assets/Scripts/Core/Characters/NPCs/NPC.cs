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
        public CharacterID characterID;
        
        public event Action<Character, CharacterID> OnInteractedWith = delegate { };

        public void OnInteract(Character character)
        {
            OnInteractedWith?.Invoke(character, characterID);
        }

        public void OnTargeted()
        {
        }

        public void OnUntargeted()
        {
        }
    }
}