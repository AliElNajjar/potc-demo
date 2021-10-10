using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK
{
    public class TOCKEditorWindow : EditorWindow
    {

        #region Variables

        private GUISkin skin;
        private readonly Color proColor = new Color(0.9f, 0.9f, 0.9f, 1);
        private readonly Color freeColor = new Color(0.1f, 0.1f, 0.1f, 1);
        private Texture2D expandedIcon, collapsedIcon;
        private static Texture2D nullsaveIcon, nullsaveWindowIcon;

        #endregion

        #region Properties

        private Color EditorColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin) return proColor;
                return freeColor;
            }
        }

        private Texture2D Icon { get; set; }

        internal GUISkin Skin
        {
            get
            {
                skin = null;
                if (skin == null)
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        skin = Resources.Load("Skins/TOCK_SkinPro") as GUISkin;
                    }
                    else
                    {
                        skin = Resources.Load("Skins/TOCK_Skin") as GUISkin;
                    }
                }

                return skin;
            }
        }

        private Texture2D ExpandedIcon
        {
            get
            {
                if (expandedIcon == null)
                {
                    expandedIcon = (Texture2D)Resources.Load("Skins/tock_expanded");
                }
                return expandedIcon;
            }
        }

        private Texture2D CollapsedIcon
        {
            get
            {
                if (collapsedIcon == null)
                {
                    collapsedIcon = (Texture2D)Resources.Load("Skins/tock_collapsed");
                }
                return collapsedIcon;
            }
        }

        internal static Texture2D NullSaveIcon
        {
            get
            {
                if (nullsaveIcon == null)
                {
                    nullsaveIcon = (Texture2D)Resources.Load("Skins/nullsave-icon");
                }
                return nullsaveIcon;
            }
        }

        internal static Texture2D NullSaveWindowIcon
        {
            get
            {
                if (nullsaveWindowIcon == null)
                {
                    nullsaveWindowIcon = (Texture2D)Resources.Load("Skins/nullsave-win-icon");
                }
                return nullsaveWindowIcon;
            }
        }

        #endregion

        #region Internal Methods

        internal void MainContainerBegin(string title, string image, bool useEditorColor = true)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);

            Color c = GUI.color;

            if (Icon == null && !string.IsNullOrEmpty(image))
            {
                Icon = (Texture2D)Resources.Load(image, typeof(Texture2D));
            }

            if (Icon != null)
            {
                GUI.color = useEditorColor ? EditorColor : Color.white; ;
                GUILayout.Label((Texture2D)Resources.Load(image, typeof(Texture2D)), GUILayout.MaxHeight(28), GUILayout.MaxWidth(28));
            }

            GUI.color = EditorColor;
            GUILayout.BeginVertical();
            GUILayout.Label(title, Skin.GetStyle("CogHeader"));
            GUILayout.EndVertical();

            GUILayout.Space(8);
            GUILayout.EndHorizontal();

            GUI.color = c;
        }

        internal void MainContainerEnd()
        {
            GUILayout.FlexibleSpace();
            Color c = GUI.color;
            GUI.color = EditorColor;

            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(NullSaveIcon);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("©2019-2020 NULLSAVE", Skin.GetStyle("CogFooter"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            GUILayout.EndVertical();
            GUI.color = c;
            GUILayout.EndVertical();
        }

        internal bool SectionGroup(string title, Texture2D icon, bool expand)
        {
            bool resValue = expand;

            // Top spacing
            GUILayout.Space(8);

            // Container start
            GUILayout.BeginHorizontal();

            // Expand collapse icon
            GUILayout.BeginVertical();
            Color res = GUI.color;
            GUILayout.Space(5);
            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;
            GUI.color = EditorColor;
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();

            // Icon
            if (icon != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndVertical();
            }
            GUI.color = res;

            // Title
            GUILayout.BeginVertical();
            if (icon != null)
            {
                GUILayout.Space(2);
            }

            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            // Container End
            GUILayout.EndHorizontal();

            // Toggle
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal void SectionHeader(string title)
        {
            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        internal bool SectionToggle(ref int displayFlags, int flag, string title, Texture2D icon = null)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
            }

            return hasFlag;
        }

        internal bool SimpleEditorBool(string title, bool value)
        {
            return EditorGUILayout.Toggle(title, value);
        }

        internal int SimpleEditorDropDown(string title, int value, string[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            int result = EditorGUILayout.Popup(value, options);
            GUILayout.EndHorizontal();
            return result;
        }

        internal float SimpleEditorFloat(string title, float value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            float result = EditorGUILayout.FloatField(value);
            GUILayout.EndHorizontal();
            return result;
        }

        internal GameObject SimpleEditorGameObject(string title, GameObject value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            GameObject result = (GameObject)EditorGUILayout.ObjectField(value, typeof(GameObject), true);
            GUILayout.EndHorizontal();
            return result;
        }

        internal int SimpleEditorInt(string title, int value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            int result = EditorGUILayout.IntField(value);
            GUILayout.EndHorizontal();
            return result;
        }

        internal void SimpleEditorLabel(string text)
        {
            GUILayout.Label(text, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
        }

        internal void SimpleEditorLabel(string title, string value)
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            GUI.enabled = false;
            string result = EditorGUILayout.TextField(value);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        internal Object SimpleEditorObject(string title, Object value, System.Type type)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            Object result = EditorGUILayout.ObjectField(value, type, true);
            GUILayout.EndHorizontal();
            return result;
        }

        internal Sprite SimpleEditorSprite(string title, Sprite value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            Sprite result = (Sprite)EditorGUILayout.ObjectField(value, typeof(Sprite), true);
            GUILayout.EndHorizontal();
            return result;
        }

        internal string SimpleEditorText(string title, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            string result = EditorGUILayout.TextField(value);
            GUILayout.EndHorizontal();
            return result;
        }

        internal void SimpleList(SerializedObject serializedObject, string listName)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);

            for (int i = 0; i < list.arraySize; i++)
            {
                if (i < list.arraySize && i >= 0)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Empty, null, string.Empty));
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            if (list.arraySize > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label("Right-click item to remove", Skin.GetStyle("CogFooter"));
            }
            else
            {
                GUILayout.Label("{Empty}", Skin.GetStyle("CogFooter"));
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add"))
            {
                list.arraySize++;
            }
            if (GUILayout.Button("Clear")) { list.arraySize = 0; }
            GUILayout.EndHorizontal();
        }

        internal void SimpleWrappedText(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.Label(text, Skin.GetStyle("BodyText"));
            GUILayout.Space(8);
            GUILayout.EndHorizontal();
        }

        #endregion

    }
}