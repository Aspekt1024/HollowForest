using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HollowForest.Dialogue
{
    [CreateAssetMenu(menuName = "HollowForest/Data/Dialogue Config", fileName = "DialogueConfig")]
    public class DialogueConfig : ScriptableObject
    {
        [Serializable]
        public class ConversationSet
        {
            public string setGuid;
            public string setName;
            public CharacterProfile interactedCharacter;
            public List<int> requiredEvents;
            public List<int> invalidatingEvents;
            public List<Conversation> conversations;
        }
        
        [Serializable]
        public class Conversation
        {
            public string dialogueGuid;
            public string character;
            public bool isOneTime;
            public List<int> requiredEvents = new List<int>();
            public List<int> invalidatingEvents = new List<int>();
            public List<string> linkedConversations = new List<string>();
            public List<string> dialogueLines = new List<string>();

            public Conversation()
            {
                dialogueGuid = Guid.NewGuid().ToString();
            }
        }

        public List<ConversationSet> ConversationSets;

        private Data.Data data;

        public void InitAwake(Data.Data data)
        {
            this.data = data;
        }
        
        public async void GetConversationAsync(Character interactingCharacter, CharacterProfile characterInteractedWith, Action<ConversationSet, Conversation> callback)
        {
            var info = await GetConversation(interactingCharacter, characterInteractedWith);
            callback?.Invoke(info.set, info.conversation);
        }

        private struct ConversationInfo
        {
            public ConversationSet set;
            public Conversation conversation;
        }

        private Task<ConversationInfo> GetConversation(Character interactingCharacter, CharacterProfile characterInteractedWith)
        {
            foreach (var set in ConversationSets)
            {
                if (!IsConditionsMet(set, characterInteractedWith)) continue;

                var conversation = DetermineConversation(set);                
                return Task.FromResult(new ConversationInfo {set = set, conversation = conversation});
            }

            return Task.FromResult(new ConversationInfo());
        }

        private Conversation DetermineConversation(ConversationSet set)
        {
            var conversation = data.GameData.dialogue.GetCurrentConversation(set);
            if (!data.GameData.dialogue.completedDialogue.Contains(conversation.dialogueGuid))
            {
                return conversation;
            }

            // Determine non-repeating conversations and prioritise them over standard conversations
            var nonRepeatingConversations = new List<Conversation>();
            var linkedConversations = new List<Conversation>();
            foreach (var guid in conversation.linkedConversations)
            {
                var index = set.conversations.FindIndex(c => c.dialogueGuid == guid);
                if (index < 0) continue;

                if (set.conversations[index].isOneTime)
                {
                    nonRepeatingConversations.Add(set.conversations[index]);
                }
                else
                {
                    linkedConversations.Add(set.conversations[index]);
                }
            }

            foreach (var nonRepeatingConversation in nonRepeatingConversations)
            {
                if (IsConditionsMet(nonRepeatingConversation))
                {
                    return nonRepeatingConversation;
                }
            }

            foreach (var linkedConversation in linkedConversations)
            {
                if (IsConditionsMet(linkedConversation))
                {
                    return linkedConversation;
                }
            }

            return conversation;
        }

        private bool IsConditionsMet(ConversationSet set, CharacterProfile characterProfile)
        {
            if (!set.conversations.Any()) return false;
            if (characterProfile.guid != set.interactedCharacter.guid) return false;

            foreach (var requiredEvent in set.requiredEvents)
            {
                if (!data.GameData.achievedEvents.Contains(requiredEvent)) return false;
            }

            foreach (var invalidatingEvent in set.invalidatingEvents)
            {
                if (data.GameData.achievedEvents.Contains(invalidatingEvent)) return false;
            }

            return true;
        }

        private bool IsConditionsMet(Conversation conversation)
        {
            if (data.GameData.dialogue.completedDialogue.Contains(conversation.dialogueGuid))
            {
                if (conversation.isOneTime) return false;
            }
            
            foreach (var requiredEvent in conversation.requiredEvents)
            {
                if (!data.GameData.achievedEvents.Contains(requiredEvent)) return false;
            }

            foreach (var invalidatingEvent in conversation.invalidatingEvents)
            {
                if (data.GameData.achievedEvents.Contains(invalidatingEvent)) return false;
            }

            return true;
        }
    }
}