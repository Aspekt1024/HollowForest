using UnityEditor;
using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public abstract class Page<T, D> where T : TabbedEditor<T, D> where D : EditorData<D>, new()
    {
        public abstract string Title { get; }
        public VisualElement Root { get; private set; }

        protected readonly T Editor;
        
        protected Page(T editor, VisualElement root)
        {
            Editor = editor;
            AddToRoot(root);
        }
        
        private void AddToRoot(VisualElement editorRoot)
        {
            Root = new VisualElement();
            Root.AddToClassList("page");
            editorRoot.Add(Root);
            
            SetupUI(Root);
        }

        public abstract void UpdateContents();
        protected abstract void SetupUI(VisualElement root);
    }
}