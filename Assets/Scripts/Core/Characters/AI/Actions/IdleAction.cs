using UnityEngine;

namespace HollowForest.AI.States
{
    public class IdleAction : AIAction
    {
        private bool isMovingRight;
        
        protected override void OnStart()
        {
            Agent.memory.RegisterStateObserver(AIState.IsNearLeftEdge, OnNearLeftEdgeStateChanged);
            Agent.memory.RegisterStateObserver(AIState.IsNearRightEdge, OnNearRightEdgeStateChanged);

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
            Agent.memory.UnregisterStateObserver(AIState.IsNearRightEdge, OnNearRightEdgeStateChanged);
            
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