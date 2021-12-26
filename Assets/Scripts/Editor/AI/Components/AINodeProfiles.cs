using System.Collections.Generic;
using Aspekt.Editors;
using UnityEngine;

namespace HollowForest.AI
{
    public static class AINodeProfiles
    {
        public static readonly Node.StyleProfile StandardStyle = new Node.StyleProfile
        {
            selectedStyle = "action-node-selected",
            unselectedStyle = "action-node-unselected",
            activatingLink = "action-node-activating-link",
        };
        
        public static readonly Node.StyleProfile EntryNodeStyle = new Node.StyleProfile
        {
            selectedStyle = "entry-node-selected",
            unselectedStyle = "entry-node-unselected",
            activatingLink = "entry-node-activating-link",
        };
        
        public static readonly Node.StyleProfile InterruptNodeStyle = new Node.StyleProfile
        {
            selectedStyle = "interrupt-node-selected",
            unselectedStyle = "interrupt-node-unselected",
            activatingLink = "interrupt-node-activating-link",
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