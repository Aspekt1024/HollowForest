using System.Collections.Generic;
using HollowForest.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HollowForest.Shop
{
    public class ShopUI : AnimatedUI
    {
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI description;
        public VerticalLayoutGroup itemListContainer;

        public ShopItemUI shopItemPrefab;

        private readonly List<ShopItemUI> itemUIs = new List<ShopItemUI>();

        private int selectedIndex;
        
        public bool Open(List<ShopItem> items)
        {
            var numItems = 0;
            foreach (var item in items)
            {
                if (!item.IsAvailable()) continue;

                var itemUI = GetOrCreateItemUI(numItems);
                itemUI.Populate(item);

                numItems++;
            }

            if (numItems == 0)
            {
                return false;
            }

            for (int i = itemUIs.Count - 1; i >= numItems; i--)
            {
                Destroy(itemUIs[i]);
                itemUIs.RemoveAt(i);
            }

            selectedIndex = -1;
            SelectItem(0);
            Game.UI.Show<ShopUI>();
            return true;
        }

        public override void OnAcceptPressed()
        {
            // TODO purchase item, destroy ui entry and select new item
        }

        public override void OnBackPressed()
        {
            Game.UI.Hide<ShopUI>();
        }

        public override void OnUpPressed()
        {
            var newIndex = Mathf.Max(selectedIndex - 1, 0);
            SelectItem(newIndex);
        }

        public override void OnDownPressed()
        {
            var newIndex = Mathf.Min(selectedIndex + 1, itemUIs.Count - 1);
            SelectItem(newIndex);
        }

        private void SelectItem(int index)
        {
            if (selectedIndex >= 0)
            {
                itemUIs[selectedIndex].ShowUnselected();
            }
            
            selectedIndex = index;
            
            var itemUI = itemUIs[index];
            itemUI.ShowSelected();
                
            itemName.text = itemUI.ItemName;
            description.text = itemUI.ItemDescription;
        }

        private ShopItemUI GetOrCreateItemUI(int index)
        {
            if (index < itemUIs.Count)
            {
                var ui = itemUIs[index];
                ui.ShowUnselected();
                return ui;
            }
            
            var itemUI = Instantiate(shopItemPrefab, itemListContainer.transform);
            itemUIs.Add(itemUI);
            itemUI.ShowUnselected();
            return itemUI;
        }
    }
}