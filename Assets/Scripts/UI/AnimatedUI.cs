using System;
using UnityEngine;

namespace HollowForest.UI
{
    public abstract class AnimatedUI : UIBase
    {
        public float fadeTime = 1f;
        public CanvasGroup canvasGroup;

        private float fadePercent;
        private bool isTransitioning;

        private Action<bool> fadeCompleteCallback;

        public void FadeIn(Action<bool> callback)
        {
            fadeCompleteCallback = callback;
            Show();
        }

        public void FadeOut(Action<bool> callback)
        {
            fadeCompleteCallback = callback;
            Hide();
        }
        
        protected override void OnAwake()
        {
        }
        
        protected override void OnHideImmediate()
        {
            fadePercent = 0;
            isTransitioning = false;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void Update()
        {
            if (!isTransitioning) return;

            fadePercent += Time.unscaledDeltaTime / fadeTime * (IsVisible ? 1f : -1f);
            canvasGroup.alpha = fadePercent;
            if (IsVisible && fadePercent >= 1f || !IsVisible && fadePercent <= 0f)
            {
                isTransitioning = false;
                canvasGroup.interactable = IsVisible;
                canvasGroup.blocksRaycasts = IsVisible;
                fadeCompleteCallback?.Invoke(IsVisible);
                fadeCompleteCallback = null;
            }
        }
        
        public override void OnAcceptPressed()
        {
        }

        protected override bool OnShow()
        {
            isTransitioning = true;
            return true;
        }

        protected override bool OnHide()
        {
            isTransitioning = true;
            return true;
        }
    }
}