using Aspekt.Editors;
using HollowForest.Dialogue.Pages;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue
{
    public class DialogueSidePanel
    {
        private readonly DialoguePage page;
        private readonly VisualElement panel;
        
        private readonly SetInspector setInfo;
        private readonly ConversationInspector conversationInfo;

        private Node selectedNode;
        private DialogueConfig.ConversationSet currentSet;
        
        public DialogueSidePanel(DialoguePage page, VisualElement rootElement)
        {
            this.page = page;

            var setInfoElement = new VisualElement();
            var conversationInfoElement = new VisualElement();
            
            panel = new VisualElement();
            setInfo = new SetInspector(this, setInfoElement);
            conversationInfo = new ConversationInspector(conversationInfoElement, page.Editor.Config);
            
            panel.AddToClassList("side-panel");
            
            panel.Add(setInfoElement);
            panel.Add(conversationInfoElement);
            
            rootElement.Add(panel);

            setInfo.NewConversationSetSelected += OnNewConversationSetSelected;
        }


        public void RecordUndo(string message) => page.RecordDialogueUndo(message); 
            
        public void Populate(DialogueConfig config, DialogueConfig.ConversationSet currentSet)
        {
            this.currentSet = currentSet;
            setInfo.PopulateSetInfo(config, currentSet);
        }

        public void OnNodeSelected(Node node)
        {
            selectedNode = node;
            PopulateNodeInfo(node);
        }

        public void OnNodeUnselected(Node node)
        {
            if (selectedNode != node) return;
            
            selectedNode = null;
            conversationInfo.Clear();
        }

        private void PopulateNodeInfo(Node node)
        {
            conversationInfo.Clear();
            
            if (node is DialogueNode dialogueNode)
            {
                conversationInfo.PopulateConversationInfo(dialogueNode, currentSet);
            }
        }

        private void OnNewConversationSetSelected(DialogueConfig.ConversationSet newSet)
        {
            if (currentSet == newSet) return;
            
            currentSet = newSet;
            page.SelectConversationSet(newSet);
        }
    }
}