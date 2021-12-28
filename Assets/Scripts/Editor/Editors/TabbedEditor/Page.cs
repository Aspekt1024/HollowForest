using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public abstract class Page<T, D> where T : TabbedEditor<T, D> where D : EditorData<D>, new()
    {
        public abstract string Title { get; }
        public VisualElement Root { get; private set; }

        public readonly T Editor;
        
        public bool IsSetup { get; private set; }
        
        protected Page(T editor)
        {
            Editor = editor;
        }
        
        public void DrawPage(VisualElement container)
        {
            Root = new VisualElement();
            Root.AddToClassList("page");
            container.Add(Root);
            
            SetupUI(Root);
            IsSetup = true;
        }

        public abstract void UpdateContents();
        public virtual void OnClear() {}
        protected abstract void SetupUI(VisualElement root);
        
        public virtual void OnGUI() { }
    }
}