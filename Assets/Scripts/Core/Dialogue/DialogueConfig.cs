using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HollowForest.Events;
using UnityEngine;

namespace HollowForest.Dialogue
{
    [CreateAssetMenu(menuName = "HollowForest/Data/Dialogue Config", fileName = "DialogueConfig")]
    public class DialogueConfig : ScriptableObject
    {
        [Serializable]
        public struct Conversation
        {
            public CharacterID characterID;
            public List<string> dialogueLines;
            public List<GameplayEvent> requiredGameplayEvents;
            public List<DialogueEvent> requiredDialogueEvents;
            public List<GameplayEvent> invalidatingGameplayEvents;
            public List<DialogueEvent> invalidatingDialogueEvents;
            public DialogueEvent dialogueEvent;
            public bool isRepeatable;
        }

        public List<Conversation> Conversations;

        private Data.Data data;

        public void InitAwake(Data.Data data)
        {
            this.data = data;
        }
        
        public async void GetConversationAsync(Character interactingCharacter, CharacterID characterInteractedWith, Action<Conversation> callback)
        {
            var dialogue = await GetConversation(interactingCharacter, characterInteractedWith);
            callback?.Invoke(dialogue);
        }

        private Task<Conversation> GetConversation(Character interactingCharacter, CharacterID characterInteractedWith)
        {
            foreach (var conversation in Conversations)
            {
                if (conversation.characterID != characterInteractedWith) continue;
                if (!conversation.isRepeatable && data.GameData.achievedDialogueEvents.Contains(conversation.dialogueEvent)) continue;

                var isRequirementMet = true;
                foreach (var invalidatingGameplayEvent in conversation.invalidatingGameplayEvents)
                {
                    if (data.GameData.achievedGameplayEvents.Contains(invalidatingGameplayEvent))
                    {
                        isRequirementMet = false;
                        break;
                    }
                }
                
                if (!isRequirementMet) continue;
                
                foreach (var invalidatingDialogueEvent in conversation.invalidatingDialogueEvents)
                {
                    if (data.GameData.achievedDialogueEvents.Contains(invalidatingDialogueEvent))
                    {
                        isRequirementMet = false;
                        break;
                    }
                }
                
                if (!isRequirementMet) continue;

                foreach (var gameplayEvent in conversation.requiredGameplayEvents)
                {
                    if (!data.GameData.achievedGameplayEvents.Contains(gameplayEvent))
                    {
                        isRequirementMet = false;
                        break;
                    }
                }

                if (!isRequirementMet) continue;

                foreach (var dialogueEvent in conversation.requiredDialogueEvents)
                {
                    if (!data.GameData.achievedDialogueEvents.Contains(dialogueEvent))
                    {
                        isRequirementMet = false;
                        break;
                    }
                }

                if (isRequirementMet)
                {
                    return Task.FromResult(conversation);
                }
            }

            return null;
        }
    }
}