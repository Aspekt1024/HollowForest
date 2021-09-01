using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aspekt.Editors
{
    public class ContextMenu
    {
        private struct MenuItem
        {
            public GenericMenu.MenuFunction2 function;
            public string text;
        }

        private readonly List<MenuItem> menuItems = new List<MenuItem>();
        
        public void AddContextMenuItem(string label, GenericMenu.MenuFunction2 function)
        {
            menuItems.Add(new MenuItem { text = label, function = function });
        }

        public void ShowContextMenu(Vector2 mousePos)
        {
            var menu = new GenericMenu();
            foreach (var item in menuItems)
            {
                menu.AddItem(new GUIContent(item.text), false, item.function, mousePos);
            }
            menu.ShowAsContext();
        }
    }
}