using System;
using UnityEngine;

namespace HollowForest.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        public bool takesFocus;
        
        private bool isVisible;

        public abstract void OnAcceptPressed();
        
        protected abstract bool OnShow();
        protected abstract bool OnHide();
        protected abstract void OnHideImmediate();
        protected abstract void OnAwake();

        protected bool IsVisible => isVisible;

        private void Awake()
        {
            OnAwake();
            HideImmediate();
        }

        public bool Show()
        {
            if (isVisible) return false;

            var success = OnShow();
            if (success)
            {
                isVisible = true;
            }
            return success;
        }

        public bool Hide()
        {
            if (!isVisible) return false;

            var success = OnHide();
            if (success)
            {
                isVisible = false;
            }
            return success;
        }

        private void HideImmediate()
        {
            isVisible = false;
            OnHideImmediate();
        }
    }
}