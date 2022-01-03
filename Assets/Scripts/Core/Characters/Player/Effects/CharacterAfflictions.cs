using System;
using UnityEngine;

namespace HollowForest.Effects
{
    public class CharacterAfflictions
    {
        [Serializable]
        public class Settings
        {
            [Header("Fall Recovery")]
            public float fallRecoveryTime = 0.5f;
        }
        
        private readonly Character character;
        private readonly Settings settings;

        private bool isRecovering;
        private float recoveryStartTime;

        public CharacterAfflictions(Character character, Settings settings)
        {
            this.character = character;
            this.settings = settings;
        }

        public void BeginFallRecovery()
        {
            character.State.SetState(CharacterStates.IsRecovering, true);
            isRecovering = true;
            recoveryStartTime = Time.time;
        }

        public void Tick()
        {
            if (isRecovering && Time.time >= recoveryStartTime + settings.fallRecoveryTime)
            {
                isRecovering = false;
                character.State.SetState(CharacterStates.IsRecovering, false);
            }
        }
    }
}