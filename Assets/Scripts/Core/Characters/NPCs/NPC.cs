using System;
using System.Linq;
using HollowForest.Effects;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest
{
    public class NPC : MonoBehaviour, IInteractive
    {
        [CharacterCategory(CharacterCategory.NPC)] public CharacterRef characterRef;
        public CharacterAnimator.Settings animatorSettings;
        public Transform model;
        public Transform indicatorPos;
        private CharacterAnimator anim;

        private CharacterProfile profile;

        private Character observedCharacter;
        
        public event Action<Character, CharacterRef> OnInteractedWith = delegate { };

        private void Awake()
        {
            anim = new CharacterAnimator(null, animatorSettings, model);
        }

        private void Start()
        {
            profile = Game.Data.Config.characterProfiles.FirstOrDefault(p => p.guid == characterRef.guid);
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
            OnInteractedWith?.Invoke(character, characterRef);
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