using HollowForest.Objects;
using TMPro;
using UnityEngine;

namespace HollowForest.Shop
{
    public class ShopItemUI : MonoBehaviour
    {
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI costText;

        private Item itemData;
        private ShopItem shopItem;

        public string ItemName => itemData.name;
        public string ItemDescription => shopItem.description;

        public void Populate(ShopItem item)
        {
            shopItem = item;
            itemData = Game.Objects.GetItemData(item.item);
            itemName.text = itemData.name;
            costText.text = item.cost.ToString();
        }

        public void ShowSelected()
        {
            itemName.text = "> " + itemData.name;
        }

        public void ShowUnselected()
        {
            if (itemData == null) return;
            itemName.text = itemData.name;
        }
    }
}