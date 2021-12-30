using UnityEngine;

namespace HollowForest.AI
{
    public class ChargeAttackAction : AIAction
    {
        public float maxChargeDistance = 4f;
        public float chargeSpeed = 18f;
        public float attackDist = 2f;
        
        // TODO public bool usesDash = false;
        
        public override string DisplayName => "Charge Attack";
        public override string MenuCategory => "Attack";

        private bool isCharging;
        private float timeChargeComplete;

        private bool isHoldingAttack;
        private float timeAttackHoldComplete;

        private bool isAwaitingAttackCompletion;
        private float timeAttackComplete;

        protected override void OnInit()
        {
            var threatDistanceSensor = Agent.character.GetComponentInChildren<ThreatDistanceSensor>();
            if (threatDistanceSensor == null)
            {
                Debug.LogWarning($"{nameof(ChargeAttackAction)} requires a {nameof(threatDistanceSensor)} " +
                                 $"component somewhere in the hierarchy of the Character Prefab, but none was found. " +
                                 $"This action will not use the distance value set on the action.");
            }
            else
            {
                threatDistanceSensor.SetDistanceThreshold(maxChargeDistance + attackDist, true, true);
            }
        }

        protected override void SetupPreconditions()
        {
            AddPrecondition(AIState.HasThreat, true);
        }

        protected override void OnStart()
        {
            var posX = Agent.character.transform.position.x;
            var enemyPosX = Agent.memory.GetObject<Character>(AIObject.Threat).transform.position.x;
            
            var threatDistance = Agent.memory.GetObject<float>(AIObject.ThreatDistance);
            var isChargingRight = enemyPosX - posX > 0f;
            var requiresCharge = threatDistance > attackDist;
            
            if (requiresCharge)
            {
                var dist = Mathf.Min(maxChargeDistance, threatDistance - attackDist);

                timeChargeComplete = Time.time + dist / chargeSpeed;
                timeAttackHoldComplete = Time.time + maxChargeDistance / chargeSpeed;

                if (isChargingRight)
                {
                    Agent.character.Physics.MoveRight(chargeSpeed);
                }
                else
                {
                    Agent.character.Physics.MoveLeft(chargeSpeed);
                }

                isCharging = true;
                isHoldingAttack = false;
                isAwaitingAttackCompletion = false;
                Agent.character.Animator.SetCharging(true);
            }
            else
            {
                if (isChargingRight)
                {
                    Agent.character.Animator.LookRight();
                }
                else
                {
                    Agent.character.Animator.LookLeft();
                }
                isCharging = false;
                isHoldingAttack = true;
                isAwaitingAttackCompletion = false;
                Agent.character.Animator.SetCharging(true);
            }
        }

        protected override void OnStop()
        {
        }

        protected override void OnTick()
        {
            if (isCharging && Time.time >= timeChargeComplete)
            {
                isCharging = false;
                Agent.character.Physics.StopMoving();

                isHoldingAttack = true;
            }
            else if (isHoldingAttack && Time.time >= timeAttackHoldComplete)
            {
                isHoldingAttack = false;
                
                Agent.character.Combat.AttackHeavyRequested();
                Agent.character.Animator.SetCharging(false);
                
                isAwaitingAttackCompletion = true;
                timeAttackComplete = Time.time + 0.5f;
            }
            else if (isAwaitingAttackCompletion && Time.time >= timeAttackComplete)
            {
                isAwaitingAttackCompletion = false;
                ActionComplete();
            }
        }
    }
}