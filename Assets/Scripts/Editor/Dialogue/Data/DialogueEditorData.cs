using System;
using System.Collections.Generic;
using System.Linq;
using Aspekt.Editors;

namespace HollowForest.Dialogue
{
    [Serializable]
    public class DialogueEditorData : EditorData<DialogueEditorData>
    {
        protected override string FilePath => "Assets/Scripts/Editor/Dialogue/Data/dialogueEditorData.json";

        public string configPath;
        public List<Node> nodes = new List<Node>();
        public string conversationSetGuid;

        public override void OnNodeRemoved(string guid)
        {
            var index = nodes.FindIndex(n => n.serializableGuid == guid);
            if (index >= 0)
            {
                nodes.RemoveAt(index);
            }
        }
    }
}