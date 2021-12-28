using System;
using System.Linq;
using System.Reflection;
using Aspekt.Editors;
using UnityEditor;
using UnityEngine;

namespace HollowForest.AI
{
    public class AIModuleNodeEditor : NodeEditor
    {
        private readonly AIEditor editor;
        private readonly Action<AIAction, Vector2> newActionCallback;

        private AIModule module;
        
        public AIModuleNodeEditor(AIEditor editor, Action<AIAction, Vector2> newActionCallback)
        {
            this.editor = editor;
            this.newActionCallback = newActionCallback;
            
            var mi = typeof(ModulePage).GetMethod(nameof(CreateNewAction), BindingFlags.NonPublic | BindingFlags.Instance);
            var actionTypes = typeof(AIAction).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(AIAction)));
            foreach (var actionType in actionTypes)
            {
                var newActionMethod = mi.MakeGenericMethod(actionType);
                var action = ScriptableObject.CreateInstance(actionType) as AIAction;
                AddContextMenuItem(
                    $"Create Action/{action.MenuCategory}/{action.DisplayName}",
                    pos => newActionMethod.Invoke(this, new[] {pos})
                );
                ScriptableObject.DestroyImmediate(action);
            }
            
            AddContextMenuItem("Reset Zoom", (pos) => ResetZoom());
            AddContextMenuItem("Find Starting Node", pos => FindNodeZero());
        }
        
        public void SetupModule(AIModule module)
        {
            
        }

        public void UpdateModuleDisplay()
        {
            UpdateContents();
        }

        private void CreateNewAction<T>(object mousePos) where T : AIAction
        {
            var newAction = ScriptableObject.CreateInstance<T>();
            newAction.name = typeof(T).Name;
            newAction.guid = Guid.NewGuid().ToString();
            
            editor.RecordUndo(module, "Add new action");

            module.actions.Add(newAction);
            AssetDatabase.AddObjectToAsset(newAction, module);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(module));
            
            newActionCallback?.Invoke(newAction, (Vector2)mousePos);
        }
    }
}