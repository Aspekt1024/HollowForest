
namespace HollowForest.AI
{
    public class CharStateMessage : LogMessage
    {
        private readonly CharacterStates state;
        private readonly bool value;
        
        public CharStateMessage(CharacterStates state, bool value, LogFilter filter, Severity severity = Severity.Info) : base(filter, severity)
        {
            this.state = state;
            this.value = value;
        }

        public override bool IsAllowedByFilter(LogFilter filter)
        {
            return !filter.suppressedCharStates.Contains(state);
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