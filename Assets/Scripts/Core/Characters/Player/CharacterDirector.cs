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
        private readonly CharacterCombat combat;

        private bool isBlocked;
        private bool isBlockTemporary;
        private float timeUnblocked;

        public CharacterDirector(Character character, CharacterPhysics physics, CharacterCombat combat)
        {
            this.character = character;
            this.physics = physics;
            this.combat = combat;
        }

        public void Tick()
        {
            if (isBlocked && isBlockTemporary && Time.time >= timeUnblocked)
            {
                UnblockInputs();
            }
        }

        public void MoveLeft()
        {
            if (isBlocked) return;
            physics.MoveLeft();
        }

        public void MoveRight()
        {
            if (isBlocked) return;
            physics.MoveRight();
        }

        public void StopMoving()
        {
            if (isBlocked) return;
            physics.StopMoving();
        }

        public void JumpPressed()
        {
            if (isBlocked) return;
            physics.JumpPressed();
        }

        public void JumpReleased()
        {
            if (isBlocked) return;
            physics.JumpReleased();
        }

        public void DashPressed()
        {
            if (isBlocked) return;
            physics.DashPressed();
        }

        public void GrapplePressed()
        {
            if (isBlocked) return;
            character.State.SetState(CharacterStates.IsGrappling, true);
        }

        public void GrappleReleased()
        {
            if (isBlocked) return;
            character.State.SetState(CharacterStates.IsGrappling, false);
        }

        public void Interact()
        {
            if (isBlocked) return;
            character.Interaction.Interact();
        }

        public void AttackLightPressed()
        {
            if (isBlocked) return;
            combat.AttackLightRequested();
        }

        public void AttackLightReleased()
        {
            if (isBlocked) return;
            combat.AttackLightReleased();
        }

        public void AttackHeavyPressed()
        {
            if (isBlocked) return;
            combat.AttackHeavyRequested();
        }

        public void AttackHeavyReleased()
        {
            if (isBlocked) return;
            combat.AttackHeavyReleased();
        }

        public void BlockInputs(float duration = -1f)
        {
            physics.BlockInput();
            combat.BlockInput();
            GrappleReleased();

            isBlocked = true;
            isBlockTemporary = duration > 0f;
            timeUnblocked = Time.time + duration;
        }

        public void UnblockInputs()
        {
            isBlocked = false;
            physics.ResumeInput();
        }
    }
}