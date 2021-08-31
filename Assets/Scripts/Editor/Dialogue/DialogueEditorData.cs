using System;
using System.Collections.Generic;
using Aspekt.Editors;

namespace HollowForest.Dialogue
{
    [Serializable]
    public class DialogueEditorData : EditorData<DialogueEditorData>
    {
        protected override string FilePath => "Assets/Scripts/Editor/Dialogue/Data/dialogueEditorData.json";

        public string configPath;
        public List<Node> nodes = new List<Node>();

    }
}