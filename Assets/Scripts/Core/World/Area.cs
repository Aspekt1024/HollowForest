using UnityEngine;

namespace HollowForest.World
{
    public class Area : MonoBehaviour
    {
        public Transform spawnPoint;
        
        public void Setup()
        {
            var npcs = GetComponentsInChildren<NPC>();
            foreach (var npc in npcs)
            {
                Game.Characters.RegisterNPC(npc);
            }
        }

        public void TearDown()
        {
            Game.Characters.UnregisterNPCs();
        }

        public void SetAtSpawnPoint(Character character)
        {
            character.transform.position = spawnPoint.transform.position;
            character.Physics.SetOnGround();
        }
    }
}