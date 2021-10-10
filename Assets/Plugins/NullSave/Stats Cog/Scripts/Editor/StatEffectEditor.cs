using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CustomEditor(typeof(StatEffect))]
    public class StatEffectEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            Behaviour = 1,
            Modifiers = 2,
            UI = 4,
        }

        #endregion

        #region Variables

        private ReorderableList modifiersList;
        private StatEffect myTarget;
        private bool showModifiers, showCancel, showPrevent;

        private DisplayFlags displayFlags;
        private Texture2D uiIcon, behaviourIcon, modifiersIcon;
        private Vector2 spMod, spPrevent, spCancel;

        #endregion

        #region Properties

        private Texture2D UIIcon
        {
            get
            {
                if (uiIcon == null)
                {
                    uiIcon = (Texture2D)Resources.Load("Icons/tock-ui", typeof(Texture2D));
                }

                return uiIcon;
            }
        }

        private Texture2D BehaviourIcon
        {
            get
            {
                if (behaviourIcon == null)
                {
                    behaviourIcon = (Texture2D)Resources.Load("Icons/tock-behaviour", typeof(Texture2D));
                }

                return behaviourIcon;
            }
        }

        private Texture2D ModifiersIcon
        {
            get
            {
                if (modifiersIcon == null)
                {
                    modifiersIcon = (Texture2D)Resources.Load("Icons/tock-tag", typeof(Texture2D));
                }

                return modifiersIcon;
            }
        }

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            if (target is StatEffect)
            {
                myTarget = (StatEffect)target;

                // Header
                modifiersList = new ReorderableList(serializedObject, serializedObject.FindProperty("modifiers"), true, true, true, true);
                modifiersList.elementHeight = (EditorGUIUtility.singleLineHeight * 9) + 22;
                modifiersList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Stat Modifiers"); };

                // Elements
                modifiersList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = modifiersList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("icon"), new GUIContent("Icon", null, string.Empty));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("displayText"), new GUIContent("Display Text", null, string.Empty));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("textColor"), new GUIContent("Text Color", null, string.Empty));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("hideInList"), new GUIContent("Hide In List", null, string.Empty));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("affectedStat"), new GUIContent("Affected Stat", null, string.Empty));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("effectType"), new GUIContent("Effect Type", null, string.Empty));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("valueTarget"), new GUIContent("Target Value", null, string.Empty));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("valueType"), new GUIContent("Value Type", null, string.Empty));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("value"), new GUIContent("Value", null, string.Empty));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                };
            }
        }

        public override void OnInspectorGUI()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBegin("Stat Effect", "Icons/statscog");

            if (SectionToggle(DisplayFlags.Behaviour, "Behaviour", BehaviourIcon))
            {
                SimpleProperty("isDetrimental", "Detrimental");
                SimpleProperty("isBenificial", "Benifecial");
                SimpleProperty("effectParticles", "Particles");
                SimpleProperty("canStack", "Stackable");
                SimpleProperty("hasLifeSpan");
                if (serializedObject.FindProperty("hasLifeSpan").boolValue)
                {
                    SimpleProperty("lifeInSeconds");
                    if (!serializedObject.FindProperty("canStack").boolValue)
                    {
                        SimpleProperty("resetLifeOnAdd");
                    }
                }
            }

            if (SectionToggle(DisplayFlags.UI, "UI", UIIcon))
            {
                SimpleProperty("category");
                SimpleProperty("sprite");
                SimpleProperty("displayName");
                SimpleProperty("description");
                SimpleProperty("startedText");
                SimpleProperty("endedText");
                SimpleProperty("removedText");
                SimpleProperty("displayInList");
            }

            if (SectionToggle(DisplayFlags.Modifiers, "Modifiers", ModifiersIcon))
            {
                modifiersList.DoLayoutList();

                SectionHeader("Cancels Events", "cancelEffects", typeof(StatEffect));
                SimpleList("cancelEffects");

                SectionHeader("Prevent Events", "preventEffects", typeof(StatEffect));
                SimpleList("preventEffects");
            }

            MainContainerEnd();

        }

        #endregion

        #region Private Methods

        void DragEffectBox(SerializedProperty list)
        {
            GUILayout.BeginVertical();
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.normal.textColor = Color.white;

            DragBox("Drag & Drop Here");

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
                            var obj = dragged as StatEffect;
                            if (obj == null)
                            {
                                continue;
                            }
                            list.arraySize++;
                            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = obj;
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            GUILayout.EndVertical();
        }

        private bool SectionToggle(DisplayFlags flag, string title, Texture2D icon)
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

        #endregion

        #region Window Methods

        internal void DrawInspector()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;

            if (SectionToggle(DisplayFlags.Behaviour, "Behaviour", BehaviourIcon))
            {
                SimpleProperty("isDetrimental", "Detrimental");
                SimpleProperty("isBenificial", "Benifecial");
                SimpleProperty("effectParticles", "Particles");
                SimpleProperty("canStack", "Stackable");
                SimpleProperty("hasLifeSpan");
                if (serializedObject.FindProperty("hasLifeSpan").boolValue)
                {
                    SimpleProperty("lifeInSeconds");
                    if (!serializedObject.FindProperty("canStack").boolValue)
                    {
                        SimpleProperty("resetLifeOnAdd");
                    }
                }
            }

            if (SectionToggle(DisplayFlags.UI, "UI", UIIcon))
            {
                SimpleProperty("category");
                SimpleProperty("sprite");
                SimpleProperty("displayName");
                SimpleProperty("description");
                SimpleProperty("startedText");
                SimpleProperty("endedText");
                SimpleProperty("removedText");
                SimpleProperty("displayInList");
            }

            if (SectionToggle(DisplayFlags.Modifiers, "Modifiers", ModifiersIcon))
            {
                modifiersList.DoLayoutList();

                SectionHeader("Cancels Events", "cancelEffects", typeof(StatEffect));
                SimpleList("cancelEffects");

                SectionHeader("Prevent Events", "preventEffects", typeof(StatEffect));
                SimpleList("preventEffects");
            }
        }

        #endregion

    }
}