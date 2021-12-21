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
        public Transform indicatorPos;

        private CharacterAnimator anim;

        private Character observedCharacter;
        
        public event Action<Character, CharacterProfile> OnInteractedWith = delegate { };

        private void Awake()
        {
            anim = new CharacterAnimator(null, animatorSettings, model);
        }

        private void Update()
        {
            WatchCharacter();
        }

        private void WatchCharacter()
        {
            if (observedCharacter ==  null) return;
            var xPos = observedCharacter.transform.position.x;
            if (xPos > transform.position.x)
            {
                anim.LookRight();    
            }
            else
            {
                anim.LookLeft();
            }
        }

        public virtual void OnInteract(Character character)
        {
            OnInteractedWith?.Invoke(character, profile);
        }

        public virtual void OnOverlap(Character character)
        {
            observedCharacter = character;
        }

        public virtual void OnOverlapEnd(Character character)
        {
            if (observedCharacter == character)
            {
                observedCharacter = null;
            }
        }

        public InteractiveOverlayDetails GetOverlayDetails()
        {
            return new InteractiveOverlayDetails
            {
                mainText = profile.characterName,
                subText = "Press [E] to talk",
                indicatorPos = indicatorPos,
            };
        }
    }
}