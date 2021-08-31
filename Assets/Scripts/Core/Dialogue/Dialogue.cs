using System;
using HollowForest.Events;
using HollowForest.UI;
using UnityEngine;

namespace HollowForest.Dialogue
{
    public class Dialogue : DialogueUI.IObserver
    {
        private UserInterface ui;
        private GameplayEvents events;
        private DialogueConfig config;

        private bool isShowingDialogue;
        private DialogueConfig.Conversation currentConversation;
        private int currentConversationLineIndex;

        private Action onDialogueCompleteCallback;

        public void InitAwake(GameplayEvents events, UserInterface ui, DialogueConfig config)
        {
            this.ui = ui;
            this.events = events;
            this.config = config;
            ui.GetUI<DialogueUI>().RegisterObserver(this);
        }

        public void InitiateDialogue(Character interactingCharacter, CharacterProfile characterInteractedWith, Action onDialogueCompleteCallback)
        {
            this.onDialogueCompleteCallback = onDialogueCompleteCallback;
            config.GetConversationAsync(interactingCharacter, characterInteractedWith, conversation =>
            {
                if (conversation == null)
                {
                    Debug.LogError($"No conversation found for {interactingCharacter} speaking to " +
                                   $"{characterInteractedWith} given current game state");
                    onDialogueCompleteCallback?.Invoke();
                    return;
                }
                BeginDialogue(conversation);
            });
            
        }

        private void BeginDialogue(DialogueConfig.Conversation conversation)
        {
            isShowingDialogue = true;
            
            currentConversation = conversation;
            currentConversationLineIndex = 0;
            
            ShowNextDialogue();
            ui.Show<DialogueUI>();
        }

        public void OnAcknowledged()
        {
            ShowNextDialogue();
        }

        public void SkipDialogue()
        {
            isShowingDialogue = false;
            ui.Hide<DialogueUI>();
            onDialogueCompleteCallback?.Invoke();
        }

        private void ShowNextDialogue()
        {
            if (currentConversationLineIndex >= currentConversation.dialogueLines.Count)
            {
                isShowingDialogue = false;
                ui.Hide<DialogueUI>();
                
                events.EventAchieved(currentConversation.achievedEventID);
                onDialogueCompleteCallback?.Invoke();
                return;
            }
            
            var dialogueUI = ui.GetUI<DialogueUI>();
            dialogueUI.SetText(currentConversation.dialogueLines[currentConversationLineIndex]);
            currentConversationLineIndex++;
        }
    }
}