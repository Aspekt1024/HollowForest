using System;
using System.Collections.Generic;
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
            public string sequenceGuid;
            public string sequenceName;
            public string interactedCharacter;
            public List<int> requiredEvents;
            public List<int> invalidatingEvents;
            public List<Conversation> conversations;
        }
        
        [Serializable]
        public class Conversation
        {
            public string conversationGuid;
            public string character;
            public int achievedEventID;
            public List<int> requiredEvents;
            public List<int> invalidatingEvents;
            public List<string> dialogueLines;
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
                
                foreach (var conversation in set.conversations)
                {
                    if (IsConditionsMet(conversation))
                    {
                        return Task.FromResult(conversation);
                    }
                }
            }

            return Task.FromResult<Conversation>(null);
        }

        private bool IsConditionsMet(ConversationSet set, CharacterProfile characterProfile)
        {
            if (characterProfile.characterName != set.interactedCharacter) return false;
            
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

            return true;
        }
    }
}