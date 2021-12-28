namespace HollowForest.AI
{
    public class LightAttackAction : AIAction
    {
        public override string DisplayName => "Light Attack";
        public override string MenuCategory => "Attack";

        protected override void SetupPreconditions()
        {
        }

        protected override void OnStart()
        {
            Character.Combat.AttackLightRequested();
        }

        protected override void OnStop()
        {
        }

        protected override void OnTick()
        {
        }
    }
}