using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HollowForest.World
{
    public class EnemySpawn : ObjectSpawn
    {
        [CharacterCategory(CharacterCategory.Enemy)] public CharacterRef enemyType;
        public bool isOneTime;
        public bool isAlwaysPresent;

        private Enemy enemy;

        public void LoadEnemy(Action<Enemy> callback)
        {
            if (isAlwaysPresent)
            {
                // TODO check if guid is registered as defeated in session data
            }

            if (isOneTime)
            {
                // TODO check if guid is registered as defeated in game data
            }
            
            var enemyData = Game.Data.Config.characterProfiles.FirstOrDefault(p => p.guid == enemyType.guid);
            if (enemyData == null)
            {
                Debug.LogError($"Failed to load enemy from character profile. Guid = {enemyType.guid}");
                callback?.Invoke(null);
                return;
            }
            
            AssetUtil.LoadAsset<Enemy>(enemyData.asset, e =>
            {
                SetupEnemy(e);
                callback?.Invoke(e);
            });
        }

        public void UnloadEnemy()
        {
            if (enemy == null) return;

            Addressables.ReleaseInstance(enemy.gameObject);
        }

        private void SetupEnemy(Enemy e)
        {
            enemy = e;
            enemy.Physics.SetOnGround(transform.position);
            
            enemy.State.RegisterStateObserver(CharacterStates.IsAlive, OnAliveStateChanged);
        }

        private void OnAliveStateChanged(bool isAlive)
        {
            if (!isAlive)
            {
                UnloadEnemy();
            }
        }
    }
}