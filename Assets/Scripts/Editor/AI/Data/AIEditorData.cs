using System.Collections.Generic;
using Aspekt.Editors;

namespace HollowForest.AI
{
    public class AIEditorData : EditorData<AIEditorData>
    {
        protected override string FilePath => "Assets/Scripts/Editor/AI/Data/aiEditorData.json";
        
        public List<Node> nodes = new List<Node>();
        public string aiModuleID;
    }
}