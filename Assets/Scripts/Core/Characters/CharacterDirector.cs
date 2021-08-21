using System;
using UnityEngine;

namespace HollowForest
{
    /// <summary>
    /// Handles directions given to the character
    /// </summary>
    public class CharacterDirector
    {

        private readonly Character character;
        private readonly CharacterPhysics physics;

        public CharacterDirector(Character character, CharacterPhysics physics)
        {
            this.character = character;
            this.physics = physics;
        }

        public void MoveLeft() => physics.MoveLeft();
        public void MoveRight() => physics.MoveRight();
        public void StopMoving() => physics.StopMoving();

        public void JumpPressed() => physics.JumpPressed();
        public void JumpReleased() => physics.JumpReleased();
    }
}