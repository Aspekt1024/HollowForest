using UnityEngine;

namespace HollowForest.Objects
{
    //[CreateAssetMenu(menuName = "Game/Item", fileName = "NewItem")]
    public abstract class ItemBehaviour : ScriptableObject
    {
        public void Collect(Item item, Character character)
        {
            Game.Events.EventAchieved(item.collectionEvent);
            OnCollected(item, character);
        }
        
        protected abstract void OnCollected(Item item, Character character);

        public abstract bool IsCollected(Item item, Character character);
    }
}