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
            public List<int> requiredEvents = new List<int>();
            public List<int> invalidatingEvents = new List<int>();
            public List<string> requiredConversations = new List<string>();
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
        
        public async void GetConversationAsync(Character interactingCharacter, CharacterProfile characterInteractedWith, Action<Conversation> callback)
        {
            var dialogue = await GetConversation(interactingCharacter, characterInteractedWith);
            callback?.Invoke(dialogue);
        }

        private Task<Conversation> GetConversation(Character interactingCharacter, CharacterProfile characterInteractedWith)
        {
            foreach (var set in ConversationSets)
            {
                if (!IsConditionsMet(set, characterInteractedWith)) continue;
                
                var conversation = set.conversations[0];
                
                const int maxIterations = 40;
                var iterations = 0;
                while (iterations < maxIterations)
                {
                    var nextConversation = GetNextConversation(conversation, set);
                    if (nextConversation != null)
                    {
                        conversation = nextConversation;
                    }
                    else
                    {
                        break;
                    }
                    iterations++;
                }
                
                return Task.FromResult(conversation);
            }

            return Task.FromResult<Conversation>(null);
        }

        private Conversation GetNextConversation(Conversation currentConversation, ConversationSet set)
        {
            foreach (var conversation in set.conversations)
            {
                if (!conversation.requiredConversations.Contains(currentConversation.dialogueGuid) || !IsConditionsMet(conversation)) continue;
                return conversation;
            }

            return null;
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
            foreach (var requiredEvent in conversation.requiredEvents)
            {
                if (!data.GameData.achievedEvents.Contains(requiredEvent)) return false;
            }

            foreach (var invalidatingEvent in conversation.invalidatingEvents)
            {
                if (data.GameData.achievedEvents.Contains(invalidatingEvent)) return false;
            }

            foreach (var c in conversation.requiredConversations)
            {
                if (!data.GameData.completedDialogue.Contains(c)) return false;
            }

            return true;
        }
    }
}