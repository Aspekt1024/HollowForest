using HollowForest.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HollowForest
{
    public class InputManager : MonoBehaviour, UserInterface.IObserver
    {
        private Character controllerCharacter;
        private UserInterface ui;

        private PlayerInput input;

        private const string PlayerMap = "Player";
        private const string UIMap = "UI";

        public void InitAwake(UserInterface ui)
        {
            this.ui = ui;
            input = GetComponent<PlayerInput>();
            
            ui.RegisterObserver(this);
        }
        
        public void SetCharacter(Character character)
        {
            controllerCharacter = character;
        }

        public void OnUIFocused()
        {
            input.SwitchCurrentActionMap(UIMap);
        }

        public void OnUIUnfocused()
        {
            input.SwitchCurrentActionMap(PlayerMap);
        }

        #region PlayerInputs

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

        private void OnDash(InputValue value)
        {
            if (value.isPressed)
            {
                controllerCharacter.Director.DashPressed();
            }
        }

        private void OnGrapple(InputValue value)
        {
            if (value.isPressed)
            {
                controllerCharacter.Director.GrapplePressed();
            }
            else
            {
                controllerCharacter.Director.GrappleReleased();
            }
        }

        private void OnInteract()
        {
            controllerCharacter.Director.Interact();
        }
        
        #endregion

        #region UIInputs

        private void OnAccept()
        {
            ui.OnAcceptReceived();
        }

        #endregion
    }
}