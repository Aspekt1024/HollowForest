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
            
            character.State.RegisterStateObserver(CharacterStates.Grounded, OnGroundedStateChanged);
        }

        public Vector2 CalculateVelocity(Vector2 currentVelocity, Vector2 requestedVelocity, float gradient)
        {
            var newVelocity = requestedVelocity;
            if (isGrounded)
            {
                if (Mathf.Abs(gradient) > 0.1f)
                {
                    newVelocity.x += currentVelocity.x - gradient;
                    newVelocity.y += currentVelocity.y + gradient;
                }
            }

            if (Mathf.Abs(newVelocity.x) < Mathf.Abs(currentVelocity.x))
            {
                newVelocity.x = Mathf.Lerp(currentVelocity.x, newVelocity.x, Time.fixedDeltaTime * 5);
            }

            if (isGrounded)
            {
                angularVelocity = -Mathf.Sign(newVelocity.x) * 2 * Mathf.PI * newVelocity.x * newVelocity.x;
            }
            else
            {
                angularVelocity = Mathf.Lerp(angularVelocity, 0f, Time.fixedDeltaTime * 0.1f);
            }
            
            character.Rigidbody.angularVelocity = angularVelocity;

            return newVelocity;
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