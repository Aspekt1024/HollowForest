using System;
using System.Linq;
using System.Threading.Tasks;
using HollowForest.AI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HollowForest.World
{
    public class EnemySpawn : ObjectSpawn
    {
        [CharacterCategory(CharacterCategory.Enemy)] public CharacterRef enemyType;
        public bool isOneTime;
        public bool isAlwaysPresent;
        public bool engageBeforeSpawn;
        public bool beginFacingRight;

        private bool canLoad;
        private Enemy enemy;

        private bool isLoadRequested;
        private bool isUnloadRequested;

        private bool isEngaged;
        private Character engagedCharacter;
        private bool isThreatLocked;

        public event Action<EnemySpawn> OnDefeated = delegate { };

        private void Start()
        {
            if (!isAlwaysPresent)
            {
                // TODO check if guid is registered as defeated in session data
            }

            if (isOneTime)
            {
                // TODO check if guid is registered as defeated in game data
            }

            canLoad = true;
        }

        private void Update()
        {
            if (!canLoad) return;
            
            var dist = transform.position - Game.Camera.mainCamera.transform.position;
            dist.z = 0f;
            var sqrMagnitude = dist.sqrMagnitude;
            const float spawnDist = 20f;
            const float despawnDist = 25f;
            
            if (sqrMagnitude >= despawnDist * despawnDist)
            {
                if (CanDespawn())
                {
                    enemy.gameObject.SetActive(false);
                }
            }
            else if (sqrMagnitude < spawnDist * spawnDist)
            {
                if (enemy == null)
                {
                    LoadEnemy(null);
                }
                else
                {
                    enemy.gameObject.SetActive(true);
                }
            }
        }

        public void Engage(Character character, bool lockOn)
        {
            isEngaged = true;
            isThreatLocked = lockOn;
            engagedCharacter = character;

            if (enemy != null)
            {
                enemy.Engage(character, lockOn);
            }
        }

        private bool CanDespawn()
        {
            if (enemy == null) return false;
            if (enemy.GetAI().memory.IsTrue(AIState.HasThreat)) return false;
            return enemy.State.GetState(CharacterStates.IsAlive);
        }

        private void LoadEnemy(Action<Enemy> callback)
        {
            if (isLoadRequested || engageBeforeSpawn && !isEngaged) return;
            isLoadRequested = true;
            
            var enemyData = Game.Data.Config.characterProfiles.FirstOrDefault(p => p.guid == enemyType.guid);
            if (enemyData == null)
            {
                Debug.LogError($"Failed to load enemy from character profile. Guid = {enemyType.guid}");
                callback?.Invoke(null);
                return;
            }
            
            AssetUtil.LoadAsset<Enemy>(enemyData.asset, e =>
            {
                if (isUnloadRequested)
                {
                    Addressables.ReleaseInstance(e.gameObject);
                    return;
                }
                SetupEnemy(e);
                callback?.Invoke(e);
            });
        }

        public void UnloadEnemy()
        {
            isUnloadRequested = true;
            if (enemy == null) return;

            Addressables.ReleaseInstance(enemy.gameObject);
            enemy = null;
        }

        private void SetupEnemy(Enemy e)
        {
            enemy = e;
            enemy.Physics.SetOnGround(transform.position);
            
            enemy.State.RegisterStateObserver(CharacterStates.IsAlive, OnAliveStateChanged);

            if (isEngaged)
            {
                enemy.Engage(engagedCharacter, isThreatLocked);
            }
            
            if (beginFacingRight)
            {
                enemy.Animator.LookRight();
            }
            else
            {
                enemy.Animator.LookLeft();
            }
        }

        private void OnAliveStateChanged(bool isAlive)
        {
            if (!isAlive)
            {
                OnDefeated?.Invoke(this);
                DeathTask();
            }
        }

        private async void DeathTask()
        {
            await Task.Delay(2500);
            UnloadEnemy();
        }
    }
}