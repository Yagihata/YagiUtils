using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using static UnityEditor.EditorGUILayout;

namespace YagihataItems.YagiUtils
{
    public class EditorGUILayoutExtra
    {

        /// <summary>
        /// インデントレベル設定を考慮した仕切り線.
        /// </summary>
        /// <param name="useIndentLevel">インデントレベルを考慮するか.</param>
        public static void Separator(bool useIndentLevel = false)
        {
            EditorGUILayout.BeginHorizontal();
            if (useIndentLevel)
            {
                GUILayout.Space(EditorGUI.indentLevel * 15);
            }
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// インデントレベルを設定する仕切り線.
        /// </summary>
        /// <param name="indentLevel">インデントレベル</param>
        public static void Separator(int indentLevel)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentLevel * 15);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.EndHorizontal();
        }
        public static void SeparatorWithSpace()
        {
            EditorGUILayoutExtra.Space();
            EditorGUILayoutExtra.Separator();
            EditorGUILayoutExtra.Space();
        }
        public static int IndexedStringList(string label, IndexedList indexedList)
        {
            EditorGUI.BeginChangeCheck();
            var property = indexedList.list;
            Func<string, int, string> selector = (string name, int number) => $"{number}: \r{name}";
            var list = indexedList.list.Select(selector).ToList();
            var divider = string.Empty;
            var unselected = "（未選択）";
            list.Add(divider);
            list.Add(unselected);
            var selectedIndex = indexedList.list.Length == 0 ? -1 : indexedList.index < 0 ? indexedList.list.Length + 1 : indexedList.index;
            var displayOptions = list.ToArray();
            var index = indexedList.list.Length > 0 ? EditorGUILayout.Popup(new GUIContent(label), selectedIndex, displayOptions) : selectedIndex;
            if (EditorGUI.EndChangeCheck())
            {
                indexedList.index = index;
            }
            indexedList.index = index > indexedList.list.Length ? -1 : index;
            return indexedList.index;
        }
        public static void Space()
        {
            EditorGUILayout.LabelField("");
        }
        //Alternate Method
        public static bool LinkLabel(string labelText)
        {
            return LinkLabel(labelText, Color.black, new Vector2(), 0);
        }

        //Alternate Method
        public static bool LinkLabel(string labelText, Color labelColor)
        {
            return LinkLabel(labelText, labelColor, new Vector2(), 0);
        }

        //Alternate Method
        public static bool LinkLabel(string labelText, Color labelColor, Vector2 contentOffset)
        {
            return LinkLabel(labelText, labelColor, contentOffset, 0);
        }

        //The Main Method
        public static bool LinkLabel(string labelText, Color labelColor, Vector2 contentOffset, int fontSize)
        {
            //Let's use Unity's label style for this
            GUIStyle stl = EditorStyles.label;
            //Next let's record the settings for Unity's label style because we will have to make sure these settings get returned back to
            //normal after we are done changing them and drawing our LinkLabel.
            Color col = stl.normal.textColor;
            Vector2 os = stl.contentOffset;
            int size = stl.fontSize;
            //Now we can modify the label's settings via the editor style : EditorStyles.label (stl).
            stl.normal.textColor = labelColor;
            stl.contentOffset = contentOffset;
            stl.fontSize = fontSize;
            //We are now ready to draw our Linklabel. I will actually use a GUILayout.Button to do this and our "stl" style will
            //make the button appear as a label.

            //Note : You may include a web address parameter in this method and open a URL at this point if the button is clicked,
            //however, I am going to just return bool based on weather or not the link was clicked. This gives me more control over
            //what actually happens when a link label is used. I also will instead include a "URL version" of this method below.

            //Since the button already returns bool, I will just return that result straight across like this.

            try
            {
                Rect rect;
                var result = false;

                using (var scope = new EditorGUILayout.VerticalScope())
                {
                    result = GUILayout.Button(labelText, stl);
                    rect = scope.rect;
                }
                rect.y += rect.height - 1;
                rect.height = 1;
                GUIStyle stl2 = GUI.skin.box;
                EditorGUI.DrawRect(rect, labelColor);
                return result;
            }
            finally
            {
                //Remember to set the editor style (stl) back to normal here. A try / finally clause will work perfectly for this!!!

                stl.normal.textColor = col;
                stl.contentOffset = os;
                stl.fontSize = size;
            }
        }

        //This is a modified version of link label that opens a URL automatically. Note : this can also return bool if you want.
        public static void LinkLabel(string labelText, Color labelColor, Vector2 contentOffset, int fontSize, string webAddress)
        {
            if (LinkLabel(labelText, labelColor, contentOffset, fontSize))
            {
                try
                {
                    Application.OpenURL(@webAddress);
                    //if returning bool, return true here.
                }
                catch
                {
                    //In most cases, the catch clause would not happen but in the interest of being thorough I will log an
                    //error and have Unity "beep" if an exception gets thrown for any reason.
                    Debug.LogError("Could not open URL. Please check your network connection and ensure the web address is correct.");
                    EditorApplication.Beep();
                }
            }
            //if returning bool, return false here.
        }
        public static void LinkLabel(string labelText, Color labelColor, Vector2 contentOffset, int fontSize, Action action)
        {
            if (LinkLabel(labelText, labelColor, contentOffset, fontSize))
            {
                try
                {
                    action();
                    //if returning bool, return true here.
                }
                catch
                {
                    //In most cases, the catch clause would not happen but in the interest of being thorough I will log an
                    //error and have Unity "beep" if an exception gets thrown for any reason.
                    Debug.LogError("Could not open URL. Please check your network connection and ensure the web address is correct.");
                    EditorApplication.Beep();
                }
            }
            //if returning bool, return false here.
        }
        public static void HeaderWithVersionInfo(Texture2D headerTexture, Rect scopeRect, Rect editorRect, string newVersion, string currentVersion, string versionPrefix)
        {
            var showingVerticalScroll = false;
            if (scopeRect.height != 0)
                showingVerticalScroll = scopeRect.height > editorRect.size.y;
            var height = editorRect.size.x / headerTexture.width * headerTexture.height;
            if (height > headerTexture.height)
                height = headerTexture.height;
            GUILayout.Box(headerTexture, GUILayout.Width(editorRect.size.x - (showingVerticalScroll ? 22 : 8)), GUILayout.Height(height));


            var rect = new Rect();
            rect.x = rect.y = 10;
            rect.width = editorRect.size.x - (showingVerticalScroll ? 22 : 8) - rect.x;
            rect.height = height - rect.y;
            using (new GUILayout.AreaScope(rect))
            {
                using (new GUILayout.VerticalScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label($"VERSION-{currentVersion} ");
                    }
                    GUILayout.FlexibleSpace();
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (newVersion.StartsWith(versionPrefix) && currentVersion != newVersion)
                        {
                            var beforeColor = GUI.backgroundColor;
                            GUI.backgroundColor = new Color(beforeColor.r, beforeColor.g, beforeColor.b, 0.7f);
                            using (new GUILayout.HorizontalScope(GUI.skin.box))
                                EditorGUILayoutExtra.LinkLabel("新しいバージョンがあります", Color.blue, new Vector2(), 0, "");
                            GUI.backgroundColor = beforeColor;
                        }
                    }
                }
            }
        }
    }
}
