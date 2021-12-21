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

        private void OnAttackLight(InputValue value)
        {
            if (value.isPressed)
            {
                controllerCharacter.Director.AttackLightPressed();
            }
            else
            {
                controllerCharacter.Director.AttackLightReleased();
            }
        }

        private void OnAttackHeavy(InputValue value)
        {
            if (value.isPressed)
            {
                controllerCharacter.Director.AttackHeavyPressed();
            }
            else
            {
                controllerCharacter.Director.AttackHeavyReleased();
            }
        }
        
        #endregion

        #region UIInputs

        private void OnAccept() => ui.OnAcceptReceived();
        private void OnBack() => ui.OnBackReceived();

        private void OnNavigate(InputValue value)
        {
            var dir = value.Get<Vector2>();
            if (dir.sqrMagnitude < 0.1f) return;
            
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (dir.x > 0)
                {
                    
                }
                else
                {
                    
                }
            }
            else
            {
                if (dir.y > 0)
                {
                    ui.OnUpReceived();
                }
                else
                {
                    ui.OnDownReceived();
                }
            }
        }

        #endregion
    }
}