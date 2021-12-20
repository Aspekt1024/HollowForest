using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HollowForest.World
{
    public class Area : MonoBehaviour
    {
        private List<ZoneArea> zoneAreas;
        
        public void Setup()
        {
            var npcs = GetComponentsInChildren<NPC>();
            foreach (var npc in npcs)
            {
                Game.Characters.RegisterNPC(npc);
            }

            zoneAreas = GetComponentsInChildren<ZoneArea>().ToList();
        }

        public void TearDown()
        {
            Game.Characters.UnregisterNPCs();
        }

        public void SetAtSpawnPoint(Character character, ZoneAreaReference zoneAreaReference)
        {
            var zoneArea = zoneAreas.FirstOrDefault(a => a.id.zoneAreaID == zoneAreaReference.zoneAreaID);
            if (zoneArea == null)
            {
                Debug.LogError($"Failed to find zone area: {zoneAreaReference.zoneAreaName}");
                return;
            }
            character.transform.position = zoneArea.spawnPoint.transform.position;
            character.Physics.SetOnGround();
        }
    }
}