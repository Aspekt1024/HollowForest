using System;
using System.Collections.Generic;

namespace HollowForest.Dialogue
{
    [Serializable]
    public class DialogueData
    {
        /// <summary>
        /// A list of all completed dialogue guids
        /// </summary>
        public List<string> completedDialogue = new List<string>();
        
        [Serializable]
        public struct DialoguePositions
        {
            public string setGuid;
            public string dialogueGuid;
        }
        
        /// <summary>
        /// A list of the current conversation id per conversation set. To access, use <see cref="GetCurrentConversation"/>
        /// </summary>
        public List<DialoguePositions> dialoguePositions = new List<DialoguePositions>();
        
        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, DialogueConfig.Conversation> conversationCache = new Dictionary<string, DialogueConfig.Conversation>(); 

        public DialogueConfig.Conversation GetCurrentConversation(DialogueConfig.ConversationSet set)
        {
            if (conversationCache.ContainsKey(set.setGuid))
            {
                return conversationCache[set.setGuid];
            }
            
            foreach (var dialoguePosition in dialoguePositions)
            {
                if (dialoguePosition.setGuid != set.setGuid) continue;
                foreach (var conversation in set.conversations)
                {
                    if (conversation.dialogueGuid != dialoguePosition.dialogueGuid) continue;
                    conversationCache.Add(set.setGuid, conversation);
                    return conversation;
                }
            }

            conversationCache.Add(set.setGuid, set.conversations[0]);
            return set.conversations[0];
        }

        public void SetCurrentConversation(DialogueConfig.ConversationSet set, DialogueConfig.Conversation conversation)
        {
            var index = dialoguePositions.FindIndex(p => p.setGuid == set.setGuid);
            if (index < 0)
            {
                dialoguePositions.Add(new DialoguePositions {setGuid = set.setGuid, dialogueGuid = conversation.dialogueGuid});
            }
            else
            {
                var pos = dialoguePositions[index];
                pos.dialogueGuid = conversation.dialogueGuid;
                dialoguePositions[index] = pos;
            }

            if (!conversationCache.ContainsKey(set.setGuid))
            {
                conversationCache.Add(set.setGuid, conversation);
            }
            else
            {
                conversationCache[set.setGuid] = conversation;
            }
        }

        public void SetDialogueComplete(DialogueConfig.ConversationSet set, DialogueConfig.Conversation conversation)
        {
            if (!completedDialogue.Contains(conversation.dialogueGuid))
            {
                completedDialogue.Add(conversation.dialogueGuid);
            }
            SetCurrentConversation(set, conversation);
        }
    }
}