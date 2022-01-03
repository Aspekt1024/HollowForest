using System;
using HollowForest.UI;
using UnityEngine;

namespace HollowForest.Dialogue
{
    public class Dialogue : DialogueUI.IObserver
    {
        private UserInterface ui;
        private DialogueConfig config;
        private DialogueData data;

        private DialogueConfig.ConversationSet currentConversationSet;
        private DialogueConfig.Conversation currentConversation;
        private int currentConversationLineIndex;

        private Action onDialogueCompleteCallback;

        public void InitAwake(DialogueConfig config, DialogueData data, UserInterface ui)
        {
            this.ui = ui;
            this.config = config;
            this.data = data;
            ui.GetUI<DialogueUI>().RegisterObserver(this);
        }

        public void InitiateDialogue(Character interactingCharacter, CharacterRef characterInteractedWith, Action onDialogueCompleteCallback)
        {
            this.onDialogueCompleteCallback = onDialogueCompleteCallback;
            config.GetConversationAsync(interactingCharacter, characterInteractedWith, (conversationSet, conversation) =>
            {
                if (conversation == null)
                {
                    onDialogueCompleteCallback?.Invoke();
                    return;
                }
                BeginDialogue(conversationSet, conversation);
            });
            
        }

        private void BeginDialogue(DialogueConfig.ConversationSet set, DialogueConfig.Conversation conversation)
        {
            currentConversationSet = set;
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
            ui.Hide<DialogueUI>();
            onDialogueCompleteCallback?.Invoke();
        }

        private void ShowNextDialogue()
        {
            if (currentConversationLineIndex >= currentConversation.dialogueLines.Count)
            {
                ui.Hide<DialogueUI>();

                data.SetDialogueComplete(currentConversationSet, currentConversation);
                onDialogueCompleteCallback?.Invoke();
                return;
            }
            
            var dialogueUI = ui.GetUI<DialogueUI>();
            dialogueUI.SetText(currentConversation.dialogueLines[currentConversationLineIndex]);
            currentConversationLineIndex++;
        }
    }
}