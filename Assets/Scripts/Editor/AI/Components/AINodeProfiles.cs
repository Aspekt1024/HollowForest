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
        public const int InterruptTransition = 1010;
        public const int DefaultTransition = 1020;
        public static readonly List<Node.DependencyProfile> DependencyProfiles = new List<Node.DependencyProfile>
        {
            new Node.DependencyProfile(ActionTransition, Color.cyan) { lineThickness = 1.5f },
            new Node.DependencyProfile(InterruptTransition, Color.magenta) { lineThickness = 1.5f },
            new Node.DependencyProfile(DefaultTransition, Color.green) { lineThickness = 1.5f },
        };
    }
}