using System;
using HollowForest.UI;
using UnityEngine;

namespace HollowForest
{
    public class Fadeout : UIBase
    {
        public float fadeTime = 1f;
        public CanvasGroup canvasGroup;

        private float fadePercent;
        private bool isTransitioning;

        public Action<bool> FadeComplete;
        
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

        protected override void OnHideImmediate()
        {
            fadePercent = 0;
            isTransitioning = false;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        protected override void OnAwake()
        {
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
                FadeComplete?.Invoke(IsVisible);
            }
        }
    }
}