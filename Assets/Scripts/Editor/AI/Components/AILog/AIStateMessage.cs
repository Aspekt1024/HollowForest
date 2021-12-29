namespace HollowForest.AI
{
    public class AIStateMessage : LogMessage
    {
        private readonly AIState state;
        private readonly bool value;
        
        public AIStateMessage(AIState state, bool value, LogFilter filter, Severity severity = Severity.Info) : base(filter, severity)
        {
            this.state = state;
            this.value = value;
        }

        public override bool IsAllowedByFilter(LogFilter filter)
        {
            return !filter.suppressedAItates.Contains(state);
        }

        protected override string GetMessage()
        {
            return $"{state} changed to {value}";
        }

        protected override void SetupContextMenu(LogFilter filter)
        {
            AddContextMenuItem($"Suppress logs for {state}", e => filter.SuppressAttribute(state));
        }
    }
}