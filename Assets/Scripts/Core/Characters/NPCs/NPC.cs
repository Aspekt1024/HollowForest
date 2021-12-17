using System;
using HollowForest.Effects;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest
{
    public class NPC : MonoBehaviour, IInteractive
    {
        public CharacterProfile profile;
        public CharacterAnimator.Settings animatorSettings;
        public Transform model;

        private CharacterAnimator anim;
        
        public event Action<Character, CharacterProfile> OnInteractedWith = delegate { };

        private void Awake()
        {
            anim = new CharacterAnimator(null, animatorSettings, model);
        }

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