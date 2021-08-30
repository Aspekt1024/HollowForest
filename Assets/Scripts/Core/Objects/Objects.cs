using System.Collections.Generic;
using UnityEngine;

namespace HollowForest.Objects
{
    public class Objects
    {
        private Game game;

        private readonly List<Item> items = new List<Item>();
        
        public Objects(Game game)
        {
            this.game = game;
            
            var itemsInScene = Object.FindObjectsOfType<Item>();
            foreach (var item in itemsInScene)
            {
                RegisterItem(item);
            }
        }
        
        public void RegisterItem(Item item)
        {
            items.Add(item);
            item.Init(game);
        }

        public void UnregisterItem(Item item)
        {
            items.Remove(item);
        }
    }
}