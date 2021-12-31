namespace HollowForest.AI
{
    public class JumpAttackAction : AIAction
    {
        public float horizontalSpeed = 3f;
        
        public override string DisplayName => "Jump Attack";
        public override string MenuCategory => "Attack";

        private bool isJumping;
        
        protected override void SetupPreconditions()
        {
            
        }

        protected override void OnStart()
        {
            if (AIUtil.IsThreatToTheRight(Agent))
            {
                Agent.character.Physics.MoveRight(horizontalSpeed);
            }
            else
            {
                Agent.character.Physics.MoveLeft(horizontalSpeed);
            }
            
            Agent.character.Physics.JumpPressed();
            Agent.character.State.RegisterStateObserver(CharacterStates.IsGrounded, OnGroundedStateChanged);
            isJumping = true;
        }

        protected override void OnStop()
        {
            Agent.character.State.UnregisterStateObserver(CharacterStates.IsGrounded, OnGroundedStateChanged);
        }

        protected override void OnTick()
        {
            
        }

        private void OnGroundedStateChanged(bool isGrounded)
        {
            if (isJumping && isGrounded)
            {
                isJumping = false;
                Agent.character.Physics.JumpReleased();
                Agent.character.Physics.StopMoving();
                Agent.character.State.UnregisterStateObserver(CharacterStates.IsGrounded, OnGroundedStateChanged);
                ActionComplete();
            }
        }
    }
}