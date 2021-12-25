using System;
using System.Collections.Generic;
using Aspekt.Editors;

namespace HollowForest.AI
{
    public class AIEditorData : EditorData<AIEditorData>
    {
        protected override string FilePath => "Assets/Scripts/Editor/AI/Data/aiEditorData.json";
        
        public List<Node> nodes = new List<Node>();
        public string aiModuleID;
        public string entryNodeGuid;
        public string interruptNodeGuid;

        public string GetEntryNodeGuid()
        {
            if (string.IsNullOrEmpty(entryNodeGuid))
            {
                entryNodeGuid = Guid.NewGuid().ToString();
            }
            return entryNodeGuid;
        }
        
        public string GetInterruptNodeGuid()
        {
            if (string.IsNullOrEmpty(interruptNodeGuid))
            {
                interruptNodeGuid = Guid.NewGuid().ToString();
            }
            return interruptNodeGuid;
        }
    }
}