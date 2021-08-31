using System;
using HollowForest.Dialogue;

namespace HollowForest
{
    [Serializable]
    public class CharacterProfile
    {
        public string guid;
        public string characterName;

        public CharacterProfile(string characterName)
        {
            guid = Guid.NewGuid().ToString();
            this.characterName = characterName;
        }
    }
}