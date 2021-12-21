using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HollowForest.UI
{
    public class UserInterface : MonoBehaviour
    {
        private List<UIBase> allUIs;
        private readonly List<UIBase> openUIs = new List<UIBase>();

        private UIBase focusedUI;
        
        public interface IObserver
        {
            void OnUIFocused();
            void OnUIUnfocused();
        }

        private readonly List<IObserver> observers = new List<IObserver>();

        public void RegisterObserver(IObserver observer) => observers.Add(observer); 

        public void InitAwake()
        {
            // TODO this means all UIs must be attached, and therefore all UI elements are loaded on startup. Most UI elements won't be needed immediately
            allUIs = GetComponentsInChildren<UIBase>().ToList();
        }

        public void OnAcceptReceived()
        {
            if (focusedUI != null) focusedUI.OnAcceptPressed();
        }

        public void OnBackReceived()
        {
            if (focusedUI != null) focusedUI.OnBackPressed();
        }

        public void OnUpReceived()
        {
            if (focusedUI != null) focusedUI.OnUpPressed();
        }

        public void OnDownReceived()
        {
            if (focusedUI != null) focusedUI.OnDownPressed();
        }

        public T GetUI<T>() where T : UIBase
        {
            var uiIndex = allUIs.FindIndex(ui => ui is T);
            if (uiIndex >= 0) return (T)allUIs[uiIndex];
            return null;
        }

        public void Show<T>() where T : UIBase
        {
            var uiIndex = allUIs.FindIndex(ui => ui is T);
            if (uiIndex < 0) return;

            var success = allUIs[uiIndex].Show();
            if (success)
            {
                openUIs.Add(allUIs[uiIndex]);
                if (allUIs[uiIndex].takesFocus)
                {
                    FocusUI(allUIs[uiIndex]);
                }
            }
        }

        public void Hide<T>() where T : UIBase
        {
            var uiIndex = openUIs.FindIndex(ui => ui is T);
            if (uiIndex < 0) return;

            var success = openUIs[uiIndex].Hide();
            if (success)
            {
                if (openUIs[uiIndex] == focusedUI)
                {
                    UnfocusUI(focusedUI);
                }
                openUIs.RemoveAt(uiIndex);
            }
        }

        private void FocusUI(UIBase ui)
        {
            focusedUI = ui;
            observers.ForEach(o => o.OnUIFocused());
        }

        private void UnfocusUI(UIBase ui)
        {
            for (int i = openUIs.Count - 1; i >= 0; i--)
            {
                if (openUIs[i] == ui || !openUIs[i].takesFocus) continue;
                focusedUI = openUIs[i];
                return;
            }
            
            focusedUI = null;
            observers.ForEach(o => o.OnUIUnfocused());
        }
    }
}