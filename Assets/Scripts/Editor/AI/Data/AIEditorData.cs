using System;
using System.Collections.Generic;
using System.Linq;
using Aspekt.Editors;
using UnityEditor;

namespace HollowForest.AI
{
    public class AIEditorData : EditorData<AIEditorData>
    {
        protected override string FilePath => "Assets/Scripts/Editor/AI/Data/aiEditorData.json";
        public string ModulesDirectory => "Assets/Config/Characters/AI";

        public AIModule selectedModule;
        [NonSerialized] public ModuleData selectedModuleData;

        [Serializable]
        public class ModuleData
        {
            public string moduleGuid;
            [NonSerialized] public AIModule module;
            public List<Node> nodes = new List<Node>();
            public string entryNodeGuid;
            public string interruptNodeGuid;

            public ModuleData(AIModule module)
            {
                this.module = module;
                moduleGuid = module.moduleGuid;
                entryNodeGuid = Guid.NewGuid().ToString();
                interruptNodeGuid = Guid.NewGuid().ToString();
            }
        }

        public List<ModuleData> moduleDataSets = new List<ModuleData>();

        public readonly List<AIModule> ModuleCache = new List<AIModule>();

        private bool isModuleReloadRequired;

        public AIEditorData()
        {
            SetupModuleCache();
        }

        public bool SetModule(AIModule module)
        {
            if (module == null) return ClearModule();
            
            var moduleData = moduleDataSets.FirstOrDefault(s => s.moduleGuid == module.moduleGuid);
            if (moduleData != null && selectedModuleData == moduleData && !isModuleReloadRequired) return false;

            isModuleReloadRequired = false;
            
            selectedModule = module;
            selectedModuleData = moduleData;
            if (selectedModuleData == null)
            {
                selectedModuleData = new ModuleData(selectedModule);
                moduleDataSets.Add(selectedModuleData);
            }

            return true;
        }

        private bool ClearModule()
        {
            if (selectedModule == null) return false;
            
            selectedModule = null;
            selectedModuleData = null;
            return true;
        }

        public void OnModulePageCleared()
        {
            isModuleReloadRequired = true;
        }

        public List<Node> GetNodes()
        {
            if (selectedModule == null) return new List<Node>();
            var moduleData = moduleDataSets.FirstOrDefault(s => s.module.moduleGuid == selectedModule.moduleGuid);
            if (moduleData.module == null)
            {
                moduleData.module = selectedModule;
            }
            return moduleData.nodes;
        }

        public string GetEntryNodeGuid()
        {
            return selectedModuleData.entryNodeGuid;
        }
        
        public string GetInterruptNodeGuid()
        {
            return selectedModuleData.interruptNodeGuid;
        }
        
        public override void OnNodeRemoved(string guid)
        {
            var index = selectedModuleData.nodes.FindIndex(n => n.serializableGuid == guid);
            if (index >= 0)
            {
                selectedModuleData.nodes.RemoveAt(index);
            }
        }

        protected override void OnPreSave()
        {
            for (int i = moduleDataSets.Count - 1; i >= 0; i--)
            {
                var moduleIndex = ModuleCache.FindIndex(m => m.moduleGuid == moduleDataSets[i].moduleGuid);
                if (moduleIndex < 0)
                {
                    moduleDataSets.RemoveAt(i);
                }
            }
        }

        protected override void OnPostLoad(AIEditorData data)
        {
            foreach (var moduleDataSet in data.moduleDataSets)
            {
                var moduleIndex = ModuleCache.FindIndex(m => m.moduleGuid == moduleDataSet.moduleGuid);
                moduleDataSet.module = moduleIndex >= 0 ? ModuleCache[moduleIndex] : null;
            }
        }

        private void SetupModuleCache()
        {
            ModuleCache.Clear();
            if (!string.IsNullOrEmpty(ModulesDirectory))
            {
                var moduleType = typeof(AIModule).FullName;
                var moduleGUIDs = AssetDatabase.FindAssets($"t:{moduleType}", new [] { ModulesDirectory });
                foreach (var moduleGUID in moduleGUIDs)
                {
                    var path = AssetDatabase.GUIDToAssetPath(moduleGUID);
                    var module = AssetDatabase.LoadAssetAtPath<AIModule>(path);
                    ModuleCache.Add(module);
                }
            }
        }
    }
}