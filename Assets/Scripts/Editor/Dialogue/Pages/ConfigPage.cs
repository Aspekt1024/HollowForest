using Aspekt.Editors;
using HollowForest.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class ConfigPage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Config";

        public ConfigPage(DialogueEditor editor, VisualElement root) : base(editor, root)
        {
        }

        public override void UpdateContents()
        {
        }

        protected override void SetupUI(VisualElement root)
        {
            if (!string.IsNullOrEmpty(Editor.Data.configPath))
            {
                Editor.Config = AssetDatabase.LoadAssetAtPath<Configuration>(Editor.Data.configPath);
            }
            
            var configPanel = CreateConfigPanel();
            root.Add(configPanel);
        }

        private VisualElement CreateConfigPanel()
        {
            var panel = new VisualElement();
            panel.AddToClassList("top-panel");

            var objectField = new ObjectField {objectType = typeof(Configuration), value = Editor.Config};
            objectField.RegisterValueChangedCallback(OnDialogueConfigChanged);
            panel.Add(objectField);

            return panel;
        }
        
        private void OnDialogueConfigChanged(ChangeEvent<UnityEngine.Object> e)
        {
            Editor.Config = e.newValue as Configuration;
            Editor.Data.configPath = AssetDatabase.GetAssetPath(Editor.Config);
            UpdateContents();
        }
    }
}