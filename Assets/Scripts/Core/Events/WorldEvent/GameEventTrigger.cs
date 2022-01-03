namespace HollowForest.Events
{
    /// <summary>
    /// Watches for an event to be triggered (e.g. from collecting an item)
    /// </summary>
    public class GameEventTrigger : EventBehaviour
    {
        public GameplayEvent endGameplayEvent;

        private WorldEvent worldEvent;
        
        public override void OnWorldEventTriggered(WorldEvent worldEvent, Character character)
        {
            this.worldEvent = worldEvent;
            Game.Events.RegisterEventObserver(endGameplayEvent, OnGameplayEventAchieved);
        }

        public override void OnWorldEventBegin()
        {
        }

        public override void OnWorldEventComplete()
        {
            Game.Events.UnregisterEventObserver(endGameplayEvent, OnGameplayEventAchieved);
        }

        private void OnGameplayEventAchieved(GameplayEvent e)
        {
            worldEvent.Complete();
        }
    }
}