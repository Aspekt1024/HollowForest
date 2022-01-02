using System;
using UnityEngine.AddressableAssets;

namespace HollowForest
{
    public enum CharacterCategory
    {
        None = 0,
        Enemy = 1000,
        NPC = 2000,
    }
    
    [Serializable]
    public class CharacterProfile
    {
        public CharacterCategory category;
        public string guid = "";
        public string characterName = "";
        public string description = "";
        public AssetReference asset;
    }

    [Serializable]
    public class CharacterRef
    {
        public string guid;
    }

    public class CharacterCategoryAttribute : System.Attribute
    {
        public readonly CharacterCategory category;
        
        public CharacterCategoryAttribute(CharacterCategory category)
        {
            this.category = category;
        }
    }
}