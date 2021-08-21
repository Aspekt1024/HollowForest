using UnityEngine;
using UnityEngine.InputSystem;

namespace HollowForest
{
    public class InputManager : MonoBehaviour
    {
        private Character controllerCharacter;

        public void SetCharacter(Character character)
        {
            controllerCharacter = character;
        }
        
        private void OnMove(InputValue value)
        {
            var movement = value.Get<Vector2>();

            if (movement.x > 0.1f)
            {
                controllerCharacter.Director.MoveRight();
            }
            else if (movement.x < -0.1)
            {
                controllerCharacter.Director.MoveLeft();
            }
            else
            {
                controllerCharacter.Director.StopMoving();
            }
        }

        private void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                controllerCharacter.Director.JumpPressed();
            }
            else
            {
                controllerCharacter.Director.JumpReleased();
            }

        }
    }
}