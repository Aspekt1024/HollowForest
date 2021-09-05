using System.Collections.Generic;
using Aspekt.Editors;
using UnityEngine;

namespace HollowForest.Dialogue
{
    public class DialogueNodeProfiles
    {
        public static readonly Node.StyleProfile StandardStyle = new Node.StyleProfile
        {
            selectedStyle = "dialogue-node-selected",
            unselectedStyle = "dialogue-node-unselected",
            activatingLink = "dialogue-node-activating-link",
        };
        
        public static readonly Node.StyleProfile OnetimeStyle = new Node.StyleProfile
        {
            selectedStyle = "dialogue-node-onetime-selected",
            unselectedStyle = "dialogue-node-onetime-unselected",
            activatingLink = "dialogue-node-onetime-activating-link",
        };

        public const int LinkDialogueDependency = 1000;
        public static readonly List<Node.DependencyProfile> DependencyProfiles = new List<Node.DependencyProfile>
        {
            new Node.DependencyProfile(LinkDialogueDependency, Color.cyan) { lineThickness = 1.5f }
        };
    }
}