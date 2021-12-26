using System;
using System.Collections.Generic;
using System.Linq;
using Aspekt.Editors;
using HollowForest.AI.States;

namespace HollowForest.AI
{
    public class AIEditorData : EditorData<AIEditorData>
    {
        protected override string FilePath => "Assets/Scripts/Editor/AI/Data/aiEditorData.json";

        public AIModule selectedModule;
        [NonSerialized] public ModuleData selectedModuleData;

        [Serializable]
        public class ModuleData
        {
            public AIModule module;
            public List<Node> nodes = new List<Node>();
            public string entryNodeGuid;
            public string interruptNodeGuid;

            public ModuleData(AIModule module)
            {
                this.module = module;
                entryNodeGuid = Guid.NewGuid().ToString();
                interruptNodeGuid = Guid.NewGuid().ToString();
            }
        }

        public List<ModuleData> moduleDataSets = new List<ModuleData>();

        public bool SetModule(AIModule module)
        {
            var moduleData = moduleDataSets.FirstOrDefault(s => s.module == module);
            if (moduleData != null && selectedModuleData == moduleData) return false;
            
            selectedModule = module;
            selectedModuleData = moduleData;
            if (selectedModuleData == null)
            {
                selectedModuleData = new ModuleData(selectedModule);
                moduleDataSets.Add(selectedModuleData);
            }

            return true;
        }

        public List<Node> GetNodes()
        {
            if (selectedModule == null) return new List<Node>();
            var moduleData = moduleDataSets.FirstOrDefault(s => s.module == selectedModule);
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
    }
}