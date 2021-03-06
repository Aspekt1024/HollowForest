using System.Collections.Generic;
using System.Linq;
using HollowForest.Objects;
using UnityEngine;

namespace HollowForest.World
{
    public class Area : MonoBehaviour
    {
        private List<ZoneArea> zoneAreas;
        private List<EnemySpawn> enemySpawns;
        
        public void Setup()
        {
            var npcs = GetComponentsInChildren<NPC>();
            foreach (var npc in npcs)
            {
                Game.Characters.RegisterNPC(npc);
            }

            var items = GetComponentsInChildren<WorldItem>();
            foreach (var item in items)
            {
                item.Init();
            }

            zoneAreas = GetComponentsInChildren<ZoneArea>().ToList();
            enemySpawns = GetComponentsInChildren<EnemySpawn>().ToList();
        }

        public void TearDown()
        {
            Game.Characters.UnregisterNPCs();
            enemySpawns.ForEach(e => e.UnloadEnemy());
        }

        public void SetAtSpawnPoint(Character character, ZoneAreaReference zoneAreaReference)
        {
            if (zoneAreaReference == null)
            {
                var spawnPoints = GetComponentsInChildren<SpawnPoint>().ToList();
                if (!spawnPoints.Any())
                {
                    Debug.LogError("Failed to find default spawn point");
                    return;
                }

                var defaultSpawn = spawnPoints.FirstOrDefault(p => p.isDefaultSpawnPoint);
                if (defaultSpawn != null)
                {
                    defaultSpawn.SetAtSpawnPoint(character);
                }
                else
                {
                    spawnPoints[0].SetAtSpawnPoint(character);
                }

                return;
            }
            
            var zoneArea = zoneAreas.FirstOrDefault(a => a.id.zoneAreaID == zoneAreaReference.zoneAreaID);
            if (zoneArea == null)
            {
                Debug.LogError($"Failed to find zone area: {zoneAreaReference.zoneAreaName}");
                return;
            }

            var spawnPos = zoneArea.spawnPoint.transform.position;
            character.Physics.SetOnGround(spawnPos);

            var isFacingRight = spawnPos.x > zoneArea.transform.position.x;
            if (isFacingRight)
            {
                character.Animator.LookRight();
            }
            else
            {
                character.Animator.LookLeft();
            }
        }
    }
}