
using UnityEngine;

namespace HollowForest.AI
{
    public class DawdleAction : AIAction
    {
        public float moveSpeed = 2f;
        public float moveDuration = 2.3f;
        public float pauseDuration = 4.7f;
        
        private bool isAtLeftEdge;
        private bool isAtRightEdge;
        
        public override string DisplayName => "Dawdle";
        public override string MenuCategory => "Idle";

        private bool isMoving;
        private float timeMovementComplete;
        private float timePauseComplete;

        private float startPosX;

        protected override void OnInit()
        {
            startPosX = Agent.character.transform.position.x;
        }

        protected override void OnStart()
        {
            Agent.memory.RegisterStateObserver(AIState.IsNearLeftEdge, OnNearLeftEdgeStateChanged);
            Agent.memory.RegisterStateObserver(AIState.IsNearLeftWall, OnNearLeftEdgeStateChanged);
            Agent.memory.RegisterStateObserver(AIState.IsNearRightEdge, OnNearRightEdgeStateChanged);
            Agent.memory.RegisterStateObserver(AIState.IsNearRightWall, OnNearRightEdgeStateChanged);

            isAtLeftEdge = Agent.memory.IsTrue(AIState.IsNearLeftEdge);
            isAtRightEdge = Agent.memory.IsTrue(AIState.IsNearRightEdge);

            BeginPause();
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
            if (isMoving)
            {
                if (Time.time >= timeMovementComplete)
                {
                    BeginPause();
                }
            }
            else
            {
                if (Time.time >= timePauseComplete)
                {
                    BeginMoving();
                }
            }
        }

        protected override void SetupPreconditions()
        {
        }

        private void BeginPause()
        {
            Agent.character.Physics.StopMoving();
            isMoving = false;
            timePauseComplete = Time.time + pauseDuration;
        }

        private void BeginMoving()
        {
            if (isAtLeftEdge)
            {
                Agent.character.Physics.MoveRight(moveSpeed);
            }
            else if (isAtRightEdge)
            {
                Agent.character.Physics.MoveLeft(moveSpeed);
            }
            else
            {
                var isMovingRight = Random.Range(0, 2) == 0;
                if (isMovingRight)
                {
                    Agent.character.Physics.MoveRight(moveSpeed);
                }
                else
                {
                    Agent.character.Physics.MoveLeft(moveSpeed);
                }
            }

            isMoving = true;
            timeMovementComplete = Time.time + moveDuration;
        }

        private void OnNearLeftEdgeStateChanged(bool isNearEdge)
        {
            isAtLeftEdge = isNearEdge;
            if (isNearEdge && isMoving)
            {
                BeginPause();
            }
        }

        private void OnNearRightEdgeStateChanged(bool isNearEdge)
        {
            isAtRightEdge = isNearEdge;
            if (isNearEdge && isMoving)
            {
                BeginPause();
            }
        }
    }
}