using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HollowForest.World
{
    public class WorldManager : MonoBehaviour
    {
        public AssetReference startArea;
        
        private Area currentArea;
        private AsyncOperationHandle<GameObject> currentHandle;

        public void InitAwake()
        {
            currentArea = GetComponentInChildren<Area>();
            if (currentArea != null)
            {
                Destroy(currentArea.gameObject);
                currentArea = null;
            }
        }

        public void Setup(Character character)
        {
            LoadArea(startArea, area =>
            {
                area.Setup();
                area.SetAtSpawnPoint(character, null);
            });
        }
        
        public void LoadArea(AssetReference assetReference, Action<Area> callback)
        {
            if (currentArea != null)
            {
                currentArea.TearDown();
                Addressables.Release(currentArea.gameObject);
                if (currentHandle.IsValid())
                {
                    Addressables.Release(currentHandle);
                }
            }
            
            currentHandle = assetReference.InstantiateAsync(transform);
            currentHandle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var area = op.Result.GetComponent<Area>();
                    if (area == null)
                    {
                        Debug.LogError($"Area was loaded, but has no {nameof(Area)} component");
                    }
                    currentArea = area;
                    callback?.Invoke(area);
                }
                else
                {
                    Debug.LogError("Failed to load area: " + assetReference.AssetGUID);
                    callback?.Invoke(null);
                }
            };
        }
    }
}