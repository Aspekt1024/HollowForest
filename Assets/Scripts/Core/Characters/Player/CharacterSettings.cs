using HollowForest.Effects;
using UnityEngine;

namespace HollowForest
{
    [CreateAssetMenu(menuName = "HollowForest/Character/Settings", fileName = "NewCharacterSettings")]
    public class CharacterSettings : ScriptableObject
    {
        public CharacterPhysics.Settings physicsSettings;
        public CharacterAfflictions.Settings afflictionSettings;
        public Health.Settings healthSettings;
        public CharacterCombat.Settings combatSettings;
    }
}