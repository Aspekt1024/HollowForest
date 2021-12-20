using System;
using System.Linq;
using Aspekt.Editors;
using HollowForest.Events;
using HollowForest.World;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HollowForest.Dialogue.Pages
{
    public class ZoneAreaPage : Page<DialogueEditor, DialogueEditorData>
    {
        public override string Title => "Zones";

        private readonly VisualElement zoneAreaContainer;
        
        public ZoneAreaPage(DialogueEditor editor) : base(editor)
        {
            zoneAreaContainer = new VisualElement();
        }
        
        public override void UpdateContents()
        {
            zoneAreaContainer.Clear();
            
            if (Editor.Config == null || Editor.Config.zoneAreaReferences == null) return;
            
            var zoneAreas = Editor.Config.zoneAreaReferences;
            foreach (var zoneArea in zoneAreas)
            {
                zoneAreaContainer.Add(CreateZoneAreaDisplay(zoneArea));
            }

            var createButton = new Button() {text = "Create New Zone Area"};
            createButton.clicked += CreateNewZoneArea;
            
            zoneAreaContainer.Add(createButton);
        }

        protected override void SetupUI(VisualElement root)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{Editor.DirectoryRoot}/Styles/Events.uss");
            root.styleSheets.Add(styleSheet);
            
            root.Add(zoneAreaContainer);
             
            UpdateContents();
        }

        private VisualElement CreateZoneAreaDisplay(ZoneAreaReference zoneArea)
        {
            var element = new VisualElement();
            element.AddToClassList("event");
            
            var zoneAreaContents = new VisualElement();
            zoneAreaContents.AddToClassList("event-contents");

            var zoneAreaDetails = new VisualElement();
            zoneAreaDetails.AddToClassList("event-details");
            var id = new Label(zoneArea.zoneAreaID.ToString());
            id.AddToClassList("event-id");
            zoneAreaDetails.Add(id);
            
            var zoneAreaName = new TextField() {value = zoneArea.zoneAreaName};
            zoneAreaName.AddToClassList("event-name");
            zoneAreaName.RegisterValueChangedCallback(e => ZoneAreaNameUpdated(zoneArea, e.newValue));
            zoneAreaDetails.Add(zoneAreaName);
            
            var description = new TextField() {value = zoneArea.description};
            description.AddToClassList("event-description");
            description.RegisterValueChangedCallback(e => ZoneAreaDescriptionUpdated(zoneArea, e.newValue));
            zoneAreaDetails.Add(description);
            
            zoneAreaContents.Add(zoneAreaDetails);
            element.Add(zoneAreaContents);

            var removeButton = new Button() {text = "Remove"};
            removeButton.AddToClassList("event-button");
            removeButton.clicked += () => RemoveZoneArea(zoneArea);
            element.Add(removeButton);

            return element;
        }

        private void ZoneAreaNameUpdated(ZoneAreaReference zoneArea, string newName)
        {
            Editor.RecordUndo(Editor.Config, "Change zone area name");
            zoneArea.zoneAreaName = newName;
        }

        private void ZoneAreaDescriptionUpdated(ZoneAreaReference zoneArea, string newDescription)
        {
            Editor.RecordUndo(Editor.Config, "Change zone area description");
            zoneArea.description = newDescription;
        }

        private int GetUniqueZoneAreaID()
        {
            var id = 10000;
            if (Editor.Config.zoneAreaReferences.Any())
            {
                id = Editor.Config.zoneAreaReferences[Editor.Config.zoneAreaReferences.Count - 1].zoneAreaID + 1;
            }
            
            return id;
        }

        private void CreateNewZoneArea()
        {
            Editor.RecordUndo(Editor.Config, "Add zone area");
                
            var uniqueZoneAreaID = GetUniqueZoneAreaID();
            Editor.Config.zoneAreaReferences.Add(new ZoneAreaReference
            {
                zoneAreaID = uniqueZoneAreaID,
                zoneAreaName = "Untitled Zone Area",
                description = "",
            });
            
            UpdateContents();
        }

        private void RemoveZoneArea(ZoneAreaReference zoneArea)
        {
            Editor.RecordUndo(Editor.Config, $"Remove zone area {zoneArea.zoneAreaID}");
            Editor.Config.zoneAreaReferences.Remove(zoneArea);
            UpdateContents();
        }
    }
}