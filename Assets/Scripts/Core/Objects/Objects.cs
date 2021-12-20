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
        }
    }
}