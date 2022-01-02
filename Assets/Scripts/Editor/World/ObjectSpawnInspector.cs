using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HollowForest.World
{
    [CustomEditor(typeof(ObjectSpawn), true)]
    public class ObjectSpawnInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate ID"))
            {
                (target as ObjectSpawn).spawnGuid = Guid.NewGuid().ToString();
            }

            if (GUILayout.Button("Check IDs"))
            {
                CheckIDs();
            }
            EditorGUILayout.EndHorizontal();
            
            base.OnInspectorGUI();
        }

        private void CheckIDs()
        {
            var area = ((ObjectSpawn) target).GetComponentInParent<Area>();
            var ids = new HashSet<string>();
            var objectSpawns = area.GetComponentsInChildren<ObjectSpawn>();
            var isValid = true;
            
            foreach (var spawn in objectSpawns)
            {
                if (string.IsNullOrEmpty(spawn.spawnGuid))
                {
                    isValid = false;
                    Debug.LogWarning($"Null spawn id: {spawn.name}");
                }
                else if (ids.Contains(spawn.spawnGuid))
                {
                    isValid = false;
                    Debug.LogWarning($"Duplicate spawn id: {spawn.name}");
                }
                else
                {
                    ids.Add(spawn.spawnGuid);
                }
            }

            if (isValid)
            {
                Debug.Log($"All spawn IDs in {area.name} are valid");
            }
        }
    }
}