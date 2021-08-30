using System;
using HollowForest.UI;
using TMPro;
using UnityEngine;

namespace HollowForest.Dialogue
{
    public class DialogueUI : UIBase
    {
        public TextMeshProUGUI dialogueText; 
            
        private Animator animator;

        private static readonly int ShowAnim = Animator.StringToHash("Show");
        private static readonly int HideAnim = Animator.StringToHash("Hide");
        
        public interface IObserver
        {
            void OnAcknowledged();
        }

        private IObserver observer;
        
        public void RegisterObserver(IObserver observer) => this.observer = observer;

        protected override void OnAwake()
        {
            animator = GetComponent<Animator>();
        }

        public void SetText(string text)
        {
            dialogueText.text = text;
        }

        public override void OnAcceptPressed()
        {
            observer?.OnAcknowledged();
        }

        protected override bool OnShow()
        {
            animator.Play(ShowAnim, 0, 0f);
            return true;
        }

        protected override bool OnHide()
        {
            animator.Play(HideAnim, 0, 0f);
            return true;
        }

        protected override void OnHideImmediate()
        {
            animator.Play(HideAnim, 0, 1f);
        }
    }
}