using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK
{
    public class TOCKEditorV2 : Editor
    {

        #region Constants

        private const string DRAG_DROP = "  Drag/Drop Here  ";

        #endregion

        #region Variables

        private GUISkin skin;
        private readonly Color proColor = new Color(0.9f, 0.9f, 0.9f, 1);
        private readonly Color freeColor = new Color(0.1f, 0.1f, 0.1f, 1);
        private Texture2D viewIcon, expandedIcon, collapsedIcon;

        internal readonly string[] bones = new string[] {"Hips", "LeftUpperLeg", "RightUpperLeg", "LeftLowerLeg", "RightLowerLeg", "LeftFoot", "RightFoot", "Spine", "Chest", "Neck", "Head",
                    "LeftShoulder", "RightShoulder", "LeftUpperArm", "RightUpperArm", "LeftLowerArm", "RightLowerArm", "LeftHand", "RightHand", "LeftToes", "RightToes", "LeftEye", "RightEye",
                    "Jaw", "LeftThumbProximal", "LeftThumbIntermediate", "LeftThumbDistal", "LeftIndexProximal", "LeftIndexIntermediate", "LeftIndexDistal", "LeftMiddleProximal",
                    "LeftMiddleIntermediate", "LeftMiddleDistal", "LeftRingProximal", "LeftRingIntermediate", "LeftRingDistal", "LeftLittleProximal", "LeftLittleIntermediate",
                    "LeftLittleDistal", "RightThumbProximal", "RightThumbIntermediate", "RightThumbDistal", "RightIndexProximal", "RightIndexIntermediate", "RightIndexDistal",
                    "RightMiddleProximal", "RightMiddleIntermediate", "RightMiddleDistal", "RightRingProximal", "RightRingIntermediate", "RightRingDistal", "RightLittleProximal",
                    "RightLittleIntermediate", "RightLittleDistal", "UpperChest", "LastBone" };

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

        internal int View { get; set; }

        private Texture2D ViewIcon
        {
            get
            {
                if (viewIcon == null)
                {
                    viewIcon = (Texture2D)Resources.Load("Icons/view", typeof(Texture2D));
                }

                return viewIcon;
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

        #endregion

        #region Internal Methods v2.1

        internal void MainContainerBeginSlim()
        {
            GUILayout.BeginVertical();
        }

        internal bool SectionToggle(int displayFlags, int flag, string title, Texture2D icon = null)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty("z_display_flags").intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        internal bool SectionDropToggle(int displayFlags, int flag, string title, Texture2D icon = null, string listName = null, System.Type acceptedType = null)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag, listName, acceptedType);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty("z_display_flags").intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        internal bool SectionGroup(string title, Texture2D icon, bool expand, string listName = null, System.Type acceptedType = null)
        {
            bool resValue = expand;
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            // Top spacing
            GUILayout.Space(8);

            // Container start
            GUILayout.BeginHorizontal();

            // Expand collapse icon
            GUILayout.BeginVertical();
            Color res = GUI.color;
            if (displayList)
            {
                GUILayout.Space(7);
            }
            else
            {
                GUILayout.Space(5);
            }
            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;
            GUI.color = EditorColor;
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();

            // Icon
            if (icon != null)
            {
                GUILayout.BeginVertical();
                if (displayList)
                {
                    GUILayout.Space(4);

                }
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndVertical();
            }
            GUI.color = res;

            // Title
            GUILayout.BeginVertical();
            if (icon != null)
            {
                if (displayList)
                {
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.Space(2);
                }
            }

            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            // Drag and drop
            if (displayList)
            {
                GUI.skin.box.alignment = TextAnchor.MiddleCenter;
                GUI.skin.box.normal.textColor = EditorColor;
                GUILayout.Box(DRAG_DROP, "box", GUILayout.ExpandWidth(true));
                if (ProcessDragDrop(list, acceptedType)) resValue = true;
            }

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

        internal void SectionHeader(string title, string listName = null, System.Type acceptedType = null)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            GUILayout.Space(displayList ? 8 : 12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            if (!displayList)
            {
                GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
                GUILayout.EndVertical();

                GUI.skin.box.alignment = TextAnchor.MiddleCenter;
                GUI.skin.box.normal.textColor = EditorColor;
                GUILayout.Box(DRAG_DROP, "box", GUILayout.ExpandWidth(true));

                ProcessDragDrop(list, acceptedType);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        internal bool SimpleBool(string propertyName)
        {
            return serializedObject.FindProperty(propertyName).boolValue;
        }

        internal void SimpleBool(string propertyName, bool value)
        {
            serializedObject.FindProperty(propertyName).boolValue = value;
        }

        internal int SimpleInt(string propertyName)
        {
            return serializedObject.FindProperty(propertyName).intValue;
        }

        internal void SimpleInt(string propertyName, int value)
        {
            serializedObject.FindProperty(propertyName).intValue = value;
        }

        internal void SubHeader(string title, string listName = null, System.Type acceptedType = null)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            GUILayout.Space(displayList ? 8 : 12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            if (!displayList)
            {
                GUILayout.Label(title, Skin.GetStyle("SubHeader"));
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.Label(title, Skin.GetStyle("SubHeader"));
                GUILayout.EndVertical();

                GUI.skin.box.alignment = TextAnchor.MiddleCenter;
                GUI.skin.box.normal.textColor = EditorColor;
                GUILayout.Box(DRAG_DROP, "box", GUILayout.ExpandWidth(true));

                ProcessDragDrop(list, acceptedType);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        #endregion

        #region Internal Methods v2

        internal void DragBox(string title)
        {
            GUILayout.BeginVertical();
            Color c = GUI.contentColor;
            GUI.contentColor = EditorColor;
            GUILayout.Box(title, "box", GUILayout.MinHeight(25), GUILayout.ExpandWidth(true));
            GUI.contentColor = c;
            GUILayout.EndVertical();
        }

        internal void DragBox(SerializedProperty list, System.Type acceptedType, string title = "  Drag/Drop Here  ")
        {
            GUILayout.BeginVertical();
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.normal.textColor = Color.white;

            DragBox(title);

            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                list.arraySize++;
                                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = dragged;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            GUILayout.EndVertical();
        }

        internal void DragDropList(SerializedProperty list, System.Type acceptedType)
        {
            DragBox(list, acceptedType);

            EditorGUILayout.Separator();

            for (int i = 0; i < list.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                if (i < list.arraySize && i >= 0)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Empty, null, string.Empty));
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
            {
                list.arraySize++;
            }

            if (GUILayout.Button("Clear"))
            {
                list.arraySize = 0;
            }
            GUILayout.EndHorizontal();
        }

        internal void MainContainerBegin(string title, string image, bool useEditorColor = true)
        {
            serializedObject.Update();
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
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
            GUILayout.Space(6);
            GUILayout.Label(title, Skin.GetStyle("CogHeader"));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.color = c;
        }

        internal void MainContainerEnd()
        {
            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("©2019-2020 NULLSAVE", Skin.GetStyle("CogFooter"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        internal bool SectionGroup(SerializedObject serializedObject, string title, string collapseVar)
        {
            bool resValue = serializedObject.FindProperty(collapseVar).boolValue;

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;

            Color res = GUI.color;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.color = new Color(1, 1, 1, .6f);
            }
            else
            {
                GUI.color = new Color(0.2f, 0.2f, 0.2f, .8f);
            }
            GUILayout.Label(texture, GUILayout.Width(12));
            GUI.color = res;

            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));

            GUILayout.EndHorizontal();

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                serializedObject.FindProperty(collapseVar).boolValue = !resValue;
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal bool SectionGroup(string title, bool expand)
        {
            bool resValue = expand;

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;

            Color res = GUI.color;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.color = new Color(1, 1, 1, .6f);
            }
            else
            {
                GUI.color = new Color(0.2f, 0.2f, 0.2f, .8f);
            }
            GUILayout.Label(texture, GUILayout.Width(12));
            GUI.color = res;

            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));

            GUILayout.EndHorizontal();

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal bool SectionGroup(string title, Texture2D icon, bool expand)
        {
            bool resValue = expand;

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;

            Color res = GUI.color;
            GUI.color = EditorColor;

            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();
            GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));

            GUI.color = res;

            GUILayout.BeginVertical();
            GUILayout.Space(2);
            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal void SimpleList(string listName)
        {
            EditorGUILayout.Separator();
            SimpleList(serializedObject.FindProperty(listName));

        }

        internal Vector2 SimpleList(string listName, Vector2 scrollPos, float maxHeight, int lineCount = 1)
        {
            EditorGUILayout.Separator();
            return SimpleList(serializedObject.FindProperty(listName), scrollPos, maxHeight, lineCount);
        }

        internal void SimpleList(string listName, System.Type acceptedType)
        {
            SimpleList(serializedObject.FindProperty(listName), acceptedType);
        }

        internal Vector2 SimpleList(string listName, System.Type acceptedType, Vector2 scrollPos, float maxHeight, int lineCount = 1)
        {
            return SimpleList(serializedObject.FindProperty(listName), acceptedType, scrollPos, maxHeight, lineCount);
        }

        internal void SimpleList(SerializedProperty list, System.Type acceptedType)
        {
            DragBox(list, acceptedType);
            EditorGUILayout.Separator();
            SimpleList(list);
        }

        internal Vector2 SimpleList(SerializedProperty list, System.Type acceptedType, Vector2 scrollPos, float maxHeight, int lineCount = 1)
        {
            DragBox(list, acceptedType);
            EditorGUILayout.Separator();
            return SimpleList(list, scrollPos, maxHeight, lineCount);
        }

        internal void SimpleMultiSelect(string propertyName)
        {
            EditorGUILayout.BeginHorizontal();
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            EditorGUILayout.LabelField(property.displayName, GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.ExpandWidth(false));
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUILayout.MaskField(property.intValue, property.enumDisplayNames);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
            EditorGUILayout.EndHorizontal();

        }

        internal void SimpleMultiSelect(string propertyName, string title)
        {
            EditorGUILayout.BeginHorizontal();
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            EditorGUILayout.LabelField(title, GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.ExpandWidth(false));
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUILayout.MaskField(property.intValue, property.enumDisplayNames);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
            EditorGUILayout.EndHorizontal();
        }

        internal void SimpleProperty(string propertyName)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName));
        }

        internal void SimpleProperty(string propertyName, string title)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), new GUIContent(title, null, string.Empty));
        }

        internal void SimplePropertyRelative(SerializedProperty property, string relativeName)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative(relativeName));
        }

        internal void SimplePropertyRelative(SerializedProperty property, string relativeName, string title)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative(relativeName), new GUIContent(title, null, string.Empty));
        }

        internal void VerticalSpace(float pixels)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(pixels);
            GUILayout.EndVertical();
        }

        internal void ViewSelect(string[] options)
        {
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUILayout.Label("View", Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();
            GUILayout.Space(4);
            Color c = GUI.color;
            GUI.color = EditorColor;
            GUILayout.Label(ViewIcon, GUILayout.MaxHeight(21), GUILayout.MaxWidth(21));
            GUI.color = c;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float oldHeight = EditorStyles.popup.fixedHeight;
            EditorStyles.popup.fixedHeight = 18;
            View = EditorGUILayout.Popup(View, options);
            EditorStyles.popup.fixedHeight = oldHeight;
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Private Methods

        private bool ProcessDragDrop(SerializedProperty list, System.Type acceptedType)
        {
            bool resValue = false;

            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                list.arraySize++;
                                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = dragged;
                                resValue = true;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            return resValue;
        }

        private void SimpleList(SerializedProperty list)
        {
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

        private Vector2 SimpleList(SerializedProperty list, Vector2 scrollPos, float maxHeight, int lineCount)
        {
            Vector2 result = Vector2.zero;
            float neededHeight = (EditorGUIUtility.singleLineHeight + 2) * lineCount * list.arraySize;
            
            if (neededHeight > maxHeight)
            {
                result = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(maxHeight));
            }

            if (list.arraySize > 0)
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    if (i < list.arraySize && i >= 0)
                    {
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Empty, null, string.Empty));
                    }
                }
            }

            if (neededHeight > maxHeight)
            {
                GUILayout.EndScrollView();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            if (list.arraySize > 0)
            {
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
                result = new Vector2(0, (EditorGUIUtility.singleLineHeight + 2) * lineCount * list.arraySize);
            }
            if (GUILayout.Button("Clear")) { list.arraySize = 0; }
            GUILayout.EndHorizontal();

            return result;
        }

        #endregion

    }
}
