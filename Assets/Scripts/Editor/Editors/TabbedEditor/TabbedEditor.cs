using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public abstract class TabbedEditor<T, D> : EditorWindow where T : TabbedEditor<T, D> where D : EditorData<D>, new()
    {
        public const string EditorRoot = "Assets/Scripts/Editor/Editors/TabbedEditor";
        public abstract string WindowName { get; }
        public D Data { get; private set; }
        
        private VisualElement root;
        
        private Toolbar<T, D> toolbar;

        private readonly List<Page<T, D>> pages = new List<Page<T, D>>();

        protected virtual void OnPreEnable() { }
        protected virtual void OnPostEnable() { }
        protected abstract void AddPages(VisualElement root);
        
        private void OnEnable()
        {
            var window = GetWindow<T>();
            window.titleContent = new GUIContent(WindowName);
            
            
            root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{EditorRoot}/Templates/TabbedEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{EditorRoot}/Templates/TabbedEditor.uss");

            visualTree.CloneTree(root);
            root.styleSheets.Add(styleSheet);

            Data ??= new D().Load();
            
            OnPreEnable();
            
            toolbar = new Toolbar<T, D>(root, Data);
            
            AddPages(root);

            toolbar.Init();
            
            Undo.undoRedoPerformed += DataFilesUpdated;
            
            OnPostEnable();
        }
        
        /// <summary>
        /// Records an object for Undo operations and displays an indicator that signifies modifications
        /// to the Upgrade Data
        /// </summary>
        public void RecordUndo(Object data, string undoMessage)
        {
            Undo.RecordObject(data, undoMessage);
            toolbar.ShowModified();
        }

        private void DataFilesUpdated()
        {
            pages.ForEach(p => p.UpdateContents());
        }
        

        private void OnDisable()
        {
            Data.Save();
            Undo.undoRedoPerformed -= DataFilesUpdated;
        }

        protected void AddPage(Page<T, D> page)
        {
            pages.Add(page);
            toolbar.AddPage(page);
        }
    }
}