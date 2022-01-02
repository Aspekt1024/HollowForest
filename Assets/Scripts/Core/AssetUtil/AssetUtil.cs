using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HollowForest
{
    public static class AssetUtil
    {
        public static void LoadAsset<T>(AssetReference assetReference, Action<T> callback) where T : Component
        {
            if (assetReference == null)
            {
                Debug.LogError("Load asset request was sent with a null asset reference");
                callback?.Invoke(null);
                return;
            }

            var handle = Addressables.InstantiateAsync(assetReference);
            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var component = op.Result.GetComponent<T>();
                    if (component == null)
                    {
                        Debug.LogError($"Asset was loaded, but has no {typeof(T).Name} component");
                    }
                    callback?.Invoke(component);
                }
                else
                {
                    Debug.LogError($"Failed to load asset of type {typeof(T).Name}. Addressable asset not found: " + assetReference.AssetGUID);
                    callback?.Invoke(null);
                }
            };
        }
    }
}