using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Aspekt.Editors
{
    [Serializable]
    public abstract class EditorData<T> where T : EditorData<T>, new()
    {
        protected abstract string FilePath { get; }
        
        public string currentPage;

        public T Load()
        {
            try
            {
                var text = File.ReadAllText(FilePath);
                var data = JsonUtility.FromJson<T>(text);
                return data;
            }
            catch
            {
                Debug.LogError($"Failed to read from {FilePath}");
                return new T();
            }
        }

        public void Save()
        {
            OnPreSave();
            var data = this as T;
            var json = JsonUtility.ToJson(data, true);
            try
            {
                var directory = Path.GetDirectoryName(FilePath);
                if (directory == null)
                {
                    throw new Exception($"No directory specified in {FilePath}");
                }
                
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.WriteAllText(FilePath, json);
            }
            catch(Exception e)
            {
                Debug.LogError($"Failed to write to {FilePath}: {e.Message}");
            }
            AssetDatabase.Refresh();
        }

        public abstract void OnNodeRemoved(string guid);

        protected virtual void OnPreSave() {}
    }
}