using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public class Toolbar<T, D> where T : TabbedEditor<T, D> where D : EditorData<D>, new()
    {
        private readonly VisualElement items;
        private readonly VisualElement modifyIndicator;
        
        private readonly List<Button> buttons = new List<Button>();
        private readonly List<Page<T, D>> pages = new List<Page<T, D>>();
        private readonly EditorData<D> data;

        private readonly ToolbarModifyIndicatorSaveProcessor toolbarModifyIndicatorSaveProcessor;

        public event Action<Page<T, D>> PageSelected = delegate { };
        
        public Toolbar(VisualElement editorRoot, EditorData<D> data)
        {
            this.data = data;

            var directoryRoot = TabbedEditor<T, D>.EditorRoot;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{directoryRoot}/Templates/Toolbar.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{directoryRoot}/Templates/Toolbar.uss");

            visualTree.CloneTree(editorRoot);
            var toolbarRoot = editorRoot.Q("Toolbar");
            toolbarRoot.styleSheets.Add(styleSheet);
            
            items = toolbarRoot.Q("ItemContainer");
            modifyIndicator = toolbarRoot.Q("ModifyIndicator");

            toolbarModifyIndicatorSaveProcessor = new ToolbarModifyIndicatorSaveProcessor(modifyIndicator);
        }

        public void Init()
        {
            if (pages.Any())
            {
                if (!string.IsNullOrEmpty(data.currentPage))
                {
                    var pageIndex = pages.FindIndex(p => p.Title == data.currentPage);
                    if (pageIndex < 0)
                    {
                        SelectDefaultPage();
                    }
                    else
                    {
                        HighlightButton(buttons[pageIndex]);
                        OnPageSelected(pages[pageIndex]);
                    }
                }
                else
                {
                    SelectDefaultPage();
                }
            }
        }

        private void SelectDefaultPage()
        {
            HighlightButton(buttons[0]);
            OnPageSelected(pages[0]);
            data.currentPage = pages[0].Title;
        }

        public void AddPage(Page<T, D> page)
        {
            pages.Add(page);
            AddPageToolbarButton(page);
        }

        public void ShowModified()
        {
            modifyIndicator.AddToClassList("modify-indicator");
        }
        
        private void AddPageToolbarButton(Page<T, D> page)
        {
            var btn = new Button { text = page.Title };
            btn.clicked += () =>
            {
                HighlightButton(btn);
                OnPageSelected(page);
            };
            items.Add(btn);
            buttons.Add(btn);
        }

        private void OnPageSelected(Page<T, D> selectedPage)
        {
            data.currentPage = selectedPage.Title;
            PageSelected?.Invoke(selectedPage);
        }

        private void HighlightButton(Button button)
        {
            const string activeClassName = "active-button";
            
            foreach (var btn in buttons)
            {
                if (btn == button)
                {
                    button.AddToClassList(activeClassName);
                }
                else
                {
                    btn.RemoveFromClassList(activeClassName);
                }
            }
        }
    }
}