using UnityEngine;

namespace HollowForest.Physics
{
    public class RollingPhysics
    {
        private readonly Character character;
        
        private bool isGrounded;
        private float angularVelocity;

        public RollingPhysics(Character character)
        {
            this.character = character;
            
            character.State.RegisterStateObserver(CharacterStates.IsGrounded, OnGroundedStateChanged);
        }

        public Vector2 CalculateVelocity(Vector2 currentVelocity, Vector2 requestedVelocity, float gradient)
        {
            var velocity = currentVelocity;
            if (requestedVelocity.x > 0.1f)
            {
                velocity.x = Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(requestedVelocity.x)) * Mathf.Sign(requestedVelocity.x);
            }
            else if (Mathf.Abs(gradient) < 0.05f)
            {
                velocity.x = Mathf.Lerp(velocity.x, requestedVelocity.x, Time.fixedDeltaTime * 0.8f);
            }
            return velocity;
        }

        private void OnGroundedStateChanged(bool isGrounded)
        {
            this.isGrounded = isGrounded;
            if (!isGrounded)
            {
            }
        }
        
    }
}