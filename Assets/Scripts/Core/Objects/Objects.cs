using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HollowForest.Objects
{
    public class Objects
    {
        private Game game;

        private readonly List<WorldItem> items = new List<WorldItem>();
        
        public Objects(Game game)
        {
            this.game = game;
        }

        public Item GetItemData(ItemRef itemRef)
        {
            var itemData = Game.Data.Config.items.FirstOrDefault(i => i.id == itemRef.id);
            if (itemData == null)
            {
                Debug.LogError("Failed to find item: " + itemRef.id);
            }
            return itemData;
        }
    }
}