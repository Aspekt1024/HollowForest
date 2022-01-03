using System;
using System.Collections.Generic;
using HollowForest.World;
using UnityEngine;

namespace HollowForest.Events
{
    public class EnemyEvent : EventBehaviour
    {
        public List<EnemySpawn> enemies;
        public float threatDelay;

        private WorldEvent worldEvent;
        private readonly HashSet<EnemySpawn> defeatedEnemy = new HashSet<EnemySpawn>();

        private bool isAwaitingThreatTrigger;
        private float threatTriggerTime;
        private Character character;
        
        public override void OnWorldEventTriggered(WorldEvent worldEvent, Character character)
        {
            this.worldEvent = worldEvent;
            this.character = character;

            foreach (var enemy in enemies)
            {
                enemy.OnDefeated += OnEnemyDefeated;
            }
        }

        public override void OnWorldEventBegin()
        {
            isAwaitingThreatTrigger = true;
            threatTriggerTime = Time.time + threatDelay;
        }

        private void Update()
        {
            if (isAwaitingThreatTrigger && Time.time >= threatTriggerTime)
            {
                isAwaitingThreatTrigger = false;
                
                foreach (var enemy in enemies)
                {
                    enemy.Engage(character, true);
                }
            }
        }

        public override void OnWorldEventComplete()
        {
            
        }

        private void OnEnemyDefeated(EnemySpawn spawn)
        {
            defeatedEnemy.Add(spawn);
            spawn.OnDefeated -= OnEnemyDefeated;
            if (defeatedEnemy.Count == enemies.Count)
            {
                worldEvent.Complete();
            }
        }
    }
}