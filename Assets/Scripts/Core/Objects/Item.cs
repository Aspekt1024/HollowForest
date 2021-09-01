using HollowForest.Events;
using HollowForest.Interactivity;
using UnityEngine;

namespace HollowForest.Objects
{
    public class Item : MonoBehaviour, IInteractive
    {
        public GameplayEvent gameplayEvent;
        
        private Game game;

        public void Init(Game game)
        {
            this.game = game;
        }
        
        public void OnInteract(Character character)
        {
            
        }

        public void OnOverlap(Character character)
        {
            game.events.EventAchieved(gameplayEvent.eventID);
            game.objects.RegisterItem(this);
            Destroy(gameObject);
        }

        public void OnOverlapEnd(Character character)
        {
        }
    }
}