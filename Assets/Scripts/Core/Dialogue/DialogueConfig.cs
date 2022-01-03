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
            public int priority;
            public List<int> requiredEvents = new List<int>();
            public List<int> invalidatingEvents = new List<int>();
            public List<Conversation> conversations = new List<Conversation>();

            public ConversationSet()
            {
                setGuid = Guid.NewGuid().ToString();
            }
        }
        
        [Serializable]
        public class Conversation
        {
            public string dialogueGuid;
            public CharacterRef character;
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
            data.Config.dialogue.ConversationSets.Sort((s1, s2) => s2.priority.CompareTo(s1.priority));
        }
        
        public async void GetConversationAsync(Character interactingCharacter, CharacterRef characterInteractedWith, Action<ConversationSet, Conversation> callback)
        {
            var info = await GetConversation(interactingCharacter, characterInteractedWith);
            callback?.Invoke(info.set, info.conversation);
        }

        private struct ConversationInfo
        {
            public ConversationSet set;
            public Conversation conversation;
        }

        private Task<ConversationInfo> GetConversation(Character interactingCharacter, CharacterRef characterInteractedWith)
        {
            // Conversations are already sorted by priority at this point, so we can return
            // on the first set that meets the current game conditions
            foreach (var set in ConversationSets)
            {
                if (!IsConditionsMet(set)) continue;

                var conversation = DetermineConversation(set, characterInteractedWith);
                if (conversation == null) continue;
                
                return Task.FromResult(new ConversationInfo {set = set, conversation = conversation});
            }

            return Task.FromResult(new ConversationInfo());
        }

        private Conversation DetermineConversation(ConversationSet set, CharacterRef characterInteractedWith)
        {
            var conversation = data.GameData.dialogue.GetCurrentConversation(set);
            if (conversation == null)
            {
                // This conversation set hasn't been accessed yet, so get the first node and see if we can run it
                if (!set.conversations.Any()) return null;
                conversation = set.conversations[0];
                return IsConditionsMet(conversation, characterInteractedWith) ? conversation : null;
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
                if (IsConditionsMet(nonRepeatingConversation, characterInteractedWith))
                {
                    return nonRepeatingConversation;
                }
            }

            foreach (var linkedConversation in linkedConversations)
            {
                if (IsConditionsMet(linkedConversation, characterInteractedWith))
                {
                    return linkedConversation;
                }
            }

            return IsConditionsMet(conversation, characterInteractedWith) ? conversation : null;
        }

        private bool IsConditionsMet(ConversationSet set)
        {
            if (!set.conversations.Any()) return false;

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

        private bool IsConditionsMet(Conversation conversation, CharacterRef characterInteractedWith)
        {
            if (conversation.character.guid != characterInteractedWith.guid) return false;
            
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