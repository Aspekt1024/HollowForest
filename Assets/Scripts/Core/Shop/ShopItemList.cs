using System.Collections.Generic;
using UnityEngine;

namespace HollowForest.Shop
{
    [CreateAssetMenu(menuName = "Hollow Forest/Shop Item List", fileName = "ShopItemList")]
    public class ShopItemList : ScriptableObject
    {
        public List<ShopItem> items;
    }
}