
namespace HollowForest.AI
{
    public class IdleAction : AIAction
    {
        private bool isMovingRight;
        
        public override string DisplayName => "Idle v1";
        public override string MenuCategory => "Idle";

        protected override void OnStart()
        {
            Agent.memory.RegisterStateObserver(AIState.IsNearLeftEdge, OnNearLeftEdgeStateChanged);
            Agent.memory.RegisterStateObserver(AIState.IsNearLeftWall, OnNearLeftEdgeStateChanged);
            Agent.memory.RegisterStateObserver(AIState.IsNearRightEdge, OnNearRightEdgeStateChanged);
            Agent.memory.RegisterStateObserver(AIState.IsNearRightWall, OnNearRightEdgeStateChanged);

            if (Agent.memory.IsTrue(AIState.IsNearLeftEdge))
            {
                isMovingRight = true;
            }
            else if (Agent.memory.IsTrue(AIState.IsNearRightEdge))
            {
                isMovingRight = false;
            }
            BeginMoving();
        }

        protected override void OnStop()
        {
            Agent.memory.UnregisterStateObserver(AIState.IsNearLeftEdge, OnNearLeftEdgeStateChanged);
            Agent.memory.UnregisterStateObserver(AIState.IsNearLeftWall, OnNearLeftEdgeStateChanged);
            Agent.memory.UnregisterStateObserver(AIState.IsNearRightEdge, OnNearRightEdgeStateChanged);
            Agent.memory.UnregisterStateObserver(AIState.IsNearRightWall, OnNearRightEdgeStateChanged);
            
            Agent.character.Director.StopMoving();
        }

        protected override void OnTick()
        {
            
        }

        protected override void SetupPreconditions()
        {
        }

        private void BeginMoving()
        {
            if (isMovingRight)
            {
                Agent.character.Director.MoveRight();
            }
            else
            {
                Agent.character.Director.MoveLeft();
            }
        }

        private void OnNearLeftEdgeStateChanged(bool isNearEdge)
        {
            if (!isNearEdge) return;
            isMovingRight = true;
            BeginMoving();
        }

        private void OnNearRightEdgeStateChanged(bool isNearEdge)
        {
            if (!isNearEdge) return;
            isMovingRight = false;
            BeginMoving();
        }
    }
}