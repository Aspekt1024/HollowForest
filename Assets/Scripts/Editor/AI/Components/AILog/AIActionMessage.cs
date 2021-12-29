namespace HollowForest.AI
{
    public class AIActionMessage : LogMessage
    {
        protected override string AdditionalMessageStyle => "action-message";

        private readonly AIExecutor.TransitionInfo info;
        
        public AIActionMessage(AIExecutor.TransitionInfo info, LogFilter filter, Severity severity = Severity.Info) : base(filter, severity)
        {
            this.info = info;
        }

        public override bool IsAllowedByFilter(LogFilter filter)
        {
            return true;
        }

        protected override string GetMessage()
        {
            if (info.isInterrupt)
            {
                return "Interrupt transition to " + info.action.DisplayName;
            }
            else
            {
                return "Transitioned to " + info.action.DisplayName;
            }
        }

        protected override void SetupContextMenu(LogFilter filter)
        {
            
        }
    }
}