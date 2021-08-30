using HollowForest.Dialogue;
using UnityEngine;

namespace HollowForest.Data
{
    [CreateAssetMenu(menuName = "HollowForest/Data/Configuration", fileName = "Configuration")]
    public class Configuration : ScriptableObject
    {
        public DialogueConfig dialogue;

        public void InitAwake(Data data)
        {
            dialogue.InitAwake(data);
        }
    }
}