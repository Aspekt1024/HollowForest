using System.Collections.Generic;

namespace HollowForest
{
    public enum CharacterAbility
    {
        None = 0,
        
        Jump = 1000,
        DoubleJump = 1010,
        
        AttachToWall = 2000,
    }
    
    public class CharacterAbilities
    {
        private readonly List<CharacterAbility> abilities = new List<CharacterAbility>();

        public bool HasAbility(CharacterAbility ability) => abilities.Contains(ability);

        public void EnableAbility(CharacterAbility ability) => abilities.Add(ability);
        public void DisableAbility(CharacterAbility ability) => abilities.Remove(ability);
    }
}