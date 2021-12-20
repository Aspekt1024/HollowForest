using System.Collections.Generic;
using HollowForest.Dialogue;
using HollowForest.Events;
using HollowForest.World;
using UnityEngine;

namespace HollowForest.Data
{
    [CreateAssetMenu(menuName = "HollowForest/Data/Configuration", fileName = "Configuration")]
    public class Configuration : ScriptableObject
    {
        public DialogueConfig dialogue;
        public List<GameplayEvent> events;
        public List<CharacterProfile> characterProfiles;
        public List<ZoneAreaReference> zoneAreaReferences;

        public void InitAwake(Data data)
        {
            dialogue.InitAwake(data);
        }

#if UNITY_EDITOR
        public static Configuration LoadConfigurationInEditor()
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Configuration>("Assets/Data/Configuration.asset");
        }
#endif
    }
}