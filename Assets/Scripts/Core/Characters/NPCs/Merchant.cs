using HollowForest.Shop;

namespace HollowForest
{
    public class Merchant : NPC
    {
        public ShopItemList itemList;
        
        public override void OnInteract(Character character)
        {
            Game.Dialogue.InitiateDialogue(character, characterRef, () =>
            {
                Game.UI.GetUI<ShopUI>().Open(itemList.items);
            });
        }
    }
}