using System;
using System.Collections.Generic;
using System.Linq;
using HollowForest.UI;

namespace HollowForest.Dialogue
{
    public class Dialogue : DialogueUI.IObserver
    {
        private UserInterface ui;

        private bool isShowingDialogue;
        private Queue<string> dialogueQueue = new Queue<string>();

        private Action onDialogueCompleteCallback;

        public void InitAwake(UserInterface ui)
        {
            this.ui = ui;
            
            ui.GetUI<DialogueUI>().RegisterObserver(this);
        }

        public void BeginDialogue(List<string> dialogueLines, Action onDialogueCompleteCallback)
        {
            isShowingDialogue = true;
            this.onDialogueCompleteCallback = onDialogueCompleteCallback;
            
            dialogueQueue = new Queue<string>(dialogueLines);
            
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
            if (!dialogueQueue.Any())
            {
                isShowingDialogue = false;
                ui.Hide<DialogueUI>();
                onDialogueCompleteCallback?.Invoke();
                return;
            }
            
            var dialogueUI = ui.GetUI<DialogueUI>();
            dialogueUI.SetText(dialogueQueue.Dequeue());
        }
    }
}