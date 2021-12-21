using UnityEngine;

namespace HollowForest.Objects
{
    [CreateAssetMenu(menuName = "Game/Ability Item", fileName = "NewAbilityItem")]
    public class AbilityItem : ItemBehaviour
    {
        public CharacterAbility ability;
        
        protected override void OnCollected(Item item, Character character)
        {
            character.Abilities.EnableAbility(ability);
        }

        public override bool IsCollected(Item item, Character character)
        {
            return character.Abilities.HasAbility(ability);
        }
    }
}