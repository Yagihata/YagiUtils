using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace YagihataItems.YagiUtils
{
    public static class TabStyle
    {
        private static GUIContent[] tabToggles = null;
        public static GUIContent[] GetTabToggles<T>() where T : struct
        {
            if (tabToggles == null)
            {
                tabToggles = Enum.GetNames(typeof(T)).Select(x => new GUIContent(x)).ToArray();
            }
            return tabToggles;
        }
        public static readonly GUIStyle TabButtonStyle = "PreButton";
        public static readonly GUI.ToolbarButtonSize TabButtonSize = GUI.ToolbarButtonSize.Fixed;
    }
}
