using UnityEngine.UIElements;

namespace Aspekt.Editors
{
    public class ToolbarModifyIndicatorSaveProcessor : UnityEditor.AssetModificationProcessor
    {
        private readonly VisualElement modifyIndicator;

        private static ToolbarModifyIndicatorSaveProcessor instance;
        
        public ToolbarModifyIndicatorSaveProcessor(VisualElement modifyIndicator)
        {
            instance = this;
            this.modifyIndicator = modifyIndicator;
        }

        protected static string[] OnWillSaveAssets(string[] paths)
        {
            instance?.modifyIndicator?.RemoveFromClassList("modify-indicator");
            return paths;
        }
    }
}