using System.Linq;
using Aspekt.Editors;
using HollowForest.Events;
using HollowForest.Objects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class ItemPage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Items";

        private readonly VisualElement container;
        
        public ItemPage(DialogueEditor editor) : base(editor)
        {
            container = new VisualElement();
        }
        
        public override void UpdateContents()
        {
            container.Clear();
            
            if (Editor.Config == null || Editor.Config.items == null) return;
            
            var items = Editor.Config.items;
            for (int i = 0; i < items.Count; i++)
            {
                container.Add(CreateItemDisplay(items[i], i));
            }

            var createButton = new Button() {text = "Create Item Instance"};
            createButton.clicked += CreateItem;
            
            container.Add(createButton);
        }

        protected override void SetupUI(VisualElement root)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Events.uss");
            root.styleSheets.Add(styleSheet);
            
            root.Add(container);
             
            UpdateContents();
        }

        private VisualElement CreateItemDisplay(Item item, int index)
        {
            var element = new VisualElement();
            element.AddToClassList("event");
            
            var contents = new VisualElement();
            contents.AddToClassList("event-contents");

            var details = new VisualElement();
            details.AddToClassList("event-details");
            var id = new Label(item.id.ToString());
            id.AddToClassList("event-id");
            details.Add(id);
            
            var name = new TextField() {value = item.name};
            name.AddToClassList("event-name");
            name.RegisterValueChangedCallback(e => NameUpdated(item, e.newValue));
            details.Add(name);
            
            var description = new TextField() {value = item.description};
            description.AddToClassList("event-description");
            description.RegisterValueChangedCallback(e => DescriptionUpdated(item, e.newValue));
            details.Add(description);

            var serializedObject = new SerializedObject(Editor.Config);
            var prop = serializedObject.FindProperty(nameof(Editor.Config.items));
            var itemProp = prop.GetArrayElementAtIndex(index);
            
            var eventProp = itemProp.FindPropertyRelative(nameof(Item.collectionEvent));
            var gameplayEvent = new PropertyField(eventProp);
            gameplayEvent.Bind(serializedObject);
            details.Add(gameplayEvent);
            
            var behaviourProp = itemProp.FindPropertyRelative(nameof(Item.behaviour));
            var behaviour = new PropertyField(behaviourProp);
            behaviour.Bind(serializedObject);
            details.Add(behaviour);
            
            contents.Add(details);
            element.Add(contents);

            var removeButton = new Button() {text = "Remove"};
            removeButton.AddToClassList("event-button");
            removeButton.clicked += () => RemoveItem(item);
            element.Add(removeButton);

            return element;
        }

        private void NameUpdated(Item item, string newName)
        {
            Editor.RecordUndo(Editor.Config, "Change item name");
            item.name = newName;
        }

        private void DescriptionUpdated(Item item, string newDescription)
        {
            Editor.RecordUndo(Editor.Config, "Change item description");
            item.description = newDescription;
        }

        private int GetUniqueID()
        {
            var id = 10000;
            if (Editor.Config.items.Any())
            {
                id = Editor.Config.items[Editor.Config.items.Count - 1].id + 1;
            }
            
            return id;
        }

        private void CreateItem()
        {
            Editor.RecordUndo(Editor.Config, "Add item");
                
            var uniqueID = GetUniqueID();
            Editor.Config.items.Add(new Item
            {
                id = uniqueID,
                name = "Untitled Item",
                description = "",
            });
            
            UpdateContents();
        }

        private void RemoveItem(Item item)
        {
            Editor.RecordUndo(Editor.Config, $"Remove item {item.id}");
            Editor.Config.items.Remove(item);
            UpdateContents();
        }
    }
}