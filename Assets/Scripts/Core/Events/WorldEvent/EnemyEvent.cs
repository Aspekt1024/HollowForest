using System;
using System.Collections.Generic;
using UnityEngine;

namespace HollowForest.Events
{
    public class EnemyEvent : EventBehaviour
    {
        public List<Enemy> enemies;
        public float threatDelay;

        private WorldEvent worldEvent;
        private readonly HashSet<Enemy> defeatedEnemy = new HashSet<Enemy>();

        private bool isAwaitingThreatTrigger;
        private float threatTriggerTime;
        private Character character;
        
        public override void OnWorldEventTriggered(WorldEvent worldEvent, Character character)
        {
            this.worldEvent = worldEvent;
            this.character = character;

            foreach (var enemy in enemies)
            {
                enemy.Defeated += OnEnemyDefeated;
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
                    enemy.Engage(character);
                }
            }
        }

        public override void OnWorldEventComplete()
        {
            
        }

        private void OnEnemyDefeated(Enemy enemy)
        {
            defeatedEnemy.Add(enemy);
            enemy.Defeated -= OnEnemyDefeated;
            if (defeatedEnemy.Count == enemies.Count)
            {
                worldEvent.Complete();
            }
        }
    }
}