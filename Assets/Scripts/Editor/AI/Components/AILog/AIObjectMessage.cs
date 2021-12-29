namespace HollowForest.AI
{
    public class AIObjectMessage : LogMessage
    {
        private readonly AIObject state;
        private readonly object value;
        
        public AIObjectMessage(AIObject state, object value, LogFilter filter, Severity severity = Severity.Info) : base(filter, severity)
        {
            this.state = state;
            this.value = value;
        }

        public override bool IsAllowedByFilter(LogFilter filter)
        {
            return !filter.suppressedAIObjects.Contains(state);
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