using System.Collections.Generic;
using Aspekt.Editors;
using UnityEngine;

namespace HollowForest.AI
{
    public class AINodeProfiles
    {
        public static readonly Node.StyleProfile StandardStyle = new Node.StyleProfile
        {
            selectedStyle = "dialogue-node-selected",
            unselectedStyle = "dialogue-node-unselected",
            activatingLink = "dialogue-node-activating-link",
        };

        public const int ActionTransition = 1000;
        public static readonly List<Node.DependencyProfile> DependencyProfiles = new List<Node.DependencyProfile>
        {
            new Node.DependencyProfile(ActionTransition, Color.cyan) { lineThickness = 1.5f }
        };
    }
}