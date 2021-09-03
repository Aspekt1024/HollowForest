using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.Characters
{
    [CustomEditor(typeof(Character))]
    public class CharacterInspector : Editor
    {
        private Character character;

        private VisualElement stateDisplay;
        private CharacterState characterState;
        
        public override VisualElement CreateInspectorGUI()
        {
            var inspector = new VisualElement();
            
            var settings = new PropertyField(serializedObject.FindProperty(nameof(character.settings)));
            var effectsSettings = new PropertyField(serializedObject.FindProperty(nameof(character.effectsSettings)));
            inspector.Add(settings);
            inspector.Add(effectsSettings);

            stateDisplay = new VisualElement();
            inspector.Add(stateDisplay);
            DrawStateDisplay();
            
            return inspector;
        }

        private void DrawStateDisplay()
        {
            stateDisplay.Clear();

            if (characterState == null) return;
            
            var stateDict = characterState.GetStateCopy();
            foreach (var state in stateDict.Keys)
            {
                var s = stateDict[state];
                var label = new Label(state + ": " + s);
                stateDisplay.Add(label);
            }
        }

        private void OnEnable()
        {
            character = target as Character;
            
            characterState = character.State;
            characterState?.RegisterAllStateObserver(OnStateChanged);
        }

        private void OnDisable()
        {
            characterState?.UnregisterAllStateObserver(OnStateChanged);
        }

        private void OnStateChanged(CharacterStates state, bool value)
        {
            DrawStateDisplay();
        }
    }
}