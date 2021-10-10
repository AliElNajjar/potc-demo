using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StatsCog))]
    public class StatsCogEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            Stats = 1,
            Effects = 2,
            Combat = 4,
            Events = 8,
            Debug = 16,
            DamageDealers = 32,
            DamageReceivers = 64,
        }

        #endregion

        #region Variables

        private StatsCog myTarget;
        private bool showStatsDebug = true;
        private bool showEffectsDebug = true;
        private int selStat;
        private string command = string.Empty;
        private Vector2 scroll;
        private DisplayFlags displayFlags;
        private Texture2D statsIcon, effectsIcon, combatIcon, eventsIcon, debugIcon, dealersIcon, receiversIcon;
        private Vector2 statSP, startingSP, resistSP;
        private string[] statOptions;

        private int boneIndex1, boneIndex2;
        private readonly string[] directions = new string[] { "FrontLeft", "FrontCenter", "FrontRight", "Left", "Right", "BackLeft", "BackCenter", "BackRight" };
        private ReorderableList stats;

        #endregion

        #region Properties

        private Texture2D StatsIcon
        {
            get
            {
                if (statsIcon == null)
                {
                    statsIcon = (Texture2D)Resources.Load("Icons/tock-stats", typeof(Texture2D));
                }

                return statsIcon;
            }
        }

        private Texture2D EffectsIcon
        {
            get
            {
                if (effectsIcon == null)
                {
                    effectsIcon = (Texture2D)Resources.Load("Icons/statscog-effect", typeof(Texture2D));
                }

                return effectsIcon;
            }
        }

        private Texture2D CombatIcon
        {
            get
            {
                if (combatIcon == null)
                {
                    combatIcon = (Texture2D)Resources.Load("Icons/statscog-combat", typeof(Texture2D));
                }

                return combatIcon;
            }
        }

        private Texture2D EventsIcon
        {
            get
            {
                if (eventsIcon == null)
                {
                    eventsIcon = (Texture2D)Resources.Load("Icons/tock-event", typeof(Texture2D));
                }

                return eventsIcon;
            }
        }

        private Texture2D DebugIcon
        {
            get
            {
                if (debugIcon == null)
                {
                    debugIcon = (Texture2D)Resources.Load("Icons/statscog-debug", typeof(Texture2D));
                }

                return debugIcon;
            }
        }

        private Texture2D DealersIcon
        {
            get
            {
                if (dealersIcon == null)
                {
                    dealersIcon = (Texture2D)Resources.Load("Icons/weapon", typeof(Texture2D));
                }

                return dealersIcon;
            }
        }

        private Texture2D ReceiversIcon
        {
            get
            {
                if (receiversIcon == null)
                {
                    receiversIcon = (Texture2D)Resources.Load("Icons/damage", typeof(Texture2D));
                }

                return receiversIcon;
            }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is StatsCog)
            {
                myTarget = (StatsCog)target;

                stats = new ReorderableList(serializedObject, serializedObject.FindProperty("stats"), true, true, true, true);
                stats.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                stats.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Stats"); };
                stats.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = stats.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
                };

            }
        }

        public override bool RequiresConstantRepaint()
        {
            if (!Application.isPlaying || View > 0) return false;
            return true;
        }

        public override void OnInspectorGUI()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBeginSlim();

            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.EndVertical();

            if (GUILayout.Button("Stats & Effects Editor", GUILayout.MinHeight(32)))
            {
                StatsWindow.Open();
            }

            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.EndVertical();


            if (Application.isPlaying)
            {
                if (SectionToggle((int)displayFlags, (int)DisplayFlags.Debug, "Debug", DebugIcon))
                {
                    DrawDebug();
                }
            }

            if (SectionDropToggle((int)displayFlags, (int)DisplayFlags.Stats, "Stats", StatsIcon, "stats", typeof(StatValue)))
            {
                statSP = SimpleList("stats", statSP, 120, 1);
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Effects, "Effects", EffectsIcon))
            {
                DrawEffects();
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Combat, "Combat", CombatIcon))
            {
                DrawCombat();
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.DamageDealers, "Damage Dealers", DealersIcon))
            {
                DamageDealer[] dealers = myTarget.GetComponentsInChildren<DamageDealer>();
                foreach (DamageDealer dealer in dealers)
                {
                    EditorGUILayout.ObjectField(dealer, typeof(DamageDealer), true);
                }
                if (dealers.Length == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    GUILayout.Label("{None}", Skin.GetStyle("SubHeader"));
                    GUILayout.EndHorizontal();
                }

                Animator anim = myTarget.GetComponentInChildren<Animator>();
                if (anim != null && anim.isHuman)
                {
                    SubHeader("Dynamic Creation");
                    boneIndex1 = EditorGUILayout.Popup("Add to Bone", boneIndex1, bones, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Add"))
                    {
                        Transform target = anim.GetBoneTransform((HumanBodyBones)boneIndex1);
                        if (target == null)
                        {
                            EditorUtility.DisplayDialog("Stats Cog", "The requested bone '" + bones[boneIndex1] + "' could not be found on the selected rig.", "OK");
                        }
                        else
                        {
                            GameObject newDD = new GameObject();
                            newDD.name = "DamageDealer_" + bones[boneIndex1];
                            newDD.AddComponent<SphereCollider>().isTrigger = true;
                            newDD.AddComponent<DamageDealer>();
                            newDD.transform.SetParent(target);
                            newDD.transform.localPosition = Vector3.zero;
                            Selection.activeGameObject = newDD;
                        }
                    }
                }
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.DamageReceivers, "Damage Receivers", ReceiversIcon))
            {
                DamageReceiver[] receivers = myTarget.GetComponentsInChildren<DamageReceiver>();
                foreach (DamageReceiver Receiver in receivers)
                {
                    EditorGUILayout.ObjectField(Receiver, typeof(DamageReceiver), true);
                }
                if (receivers.Length == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    GUILayout.Label("{None}", Skin.GetStyle("SubHeader"));
                    GUILayout.EndHorizontal();
                }

                Animator anim = myTarget.GetComponentInChildren<Animator>();
                if (anim != null && anim.isHuman)
                {
                    SubHeader("Dynamic Creation");
                    boneIndex2 = EditorGUILayout.Popup("Add to Bone", boneIndex2, bones, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Add"))
                    {
                        Transform target = anim.GetBoneTransform((HumanBodyBones)boneIndex2);
                        if (target == null)
                        {
                            EditorUtility.DisplayDialog("Stats Cog", "The requested bone '" + bones[boneIndex2] + "' could not be found on the selected rig.", "OK");
                        }
                        else
                        {
                            GameObject newDD = new GameObject();
                            newDD.name = "DamageReceiver_" + bones[boneIndex2];
                            newDD.AddComponent<CapsuleCollider>().isTrigger = true;
                            newDD.AddComponent<DamageReceiver>();
                            newDD.transform.SetParent(target);
                            newDD.transform.localPosition = Vector3.zero;
                            Selection.activeGameObject = newDD;
                        }
                    }
                }
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Events, "Events", EventsIcon))
            {
                DrawEvents();
            }

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        internal void DrawCombat()
        {
            int curOption = -1;
            if (statOptions == null || statOptions.Length != myTarget.stats.Count)
            {
                if (myTarget.stats != null)
                {
                    statOptions = new string[myTarget.stats.Count];
                    for (int i = 0; i < statOptions.Length; i++)
                    {
                        statOptions[i] = myTarget.stats[i].name;
                    }
                }
            }

            if (statOptions != null)
            {
                for (int i = 0; i < statOptions.Length; i++)
                {
                    if (statOptions[i] == myTarget.healthStat)
                    {
                        curOption = i;
                    }
                }
            }

            SectionHeader("Health");
            if (statOptions == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.Label("Add stats to select health", Skin.GetStyle("SubHeader"));
                GUILayout.EndHorizontal();
            }
            else
            {
                curOption = EditorGUILayout.Popup("Health Stat", curOption, statOptions);
                if (curOption != -1)
                {
                    serializedObject.FindProperty("healthStat").stringValue = statOptions[curOption];
                }
            }
            SimpleProperty("damageValue");
            SimpleProperty("hitDirTolerance");
            SimpleProperty("ignoreLayer");

            int immune = SimpleInt("directionImmunity");
            serializedObject.FindProperty("directionImmunity").intValue = EditorGUILayout.MaskField("Direction Immunity", immune, directions);

            SimpleProperty("immunityAfterHit");

            SectionHeader("Damage Mods", "damageModifiers", typeof(DamageModifier));
            SimpleList("damageModifiers");
        }

        private void DrawDebug()
        {
            bool tmp = showStatsDebug;
            showStatsDebug = SectionGroup("Stat Values", showStatsDebug);
            if (tmp)
            {
                if (myTarget.stats == null || myTarget.stats.Count == 0)
                {
                    EditorGUILayout.LabelField("No StatValues available to be monitored.", Skin.GetStyle("WrapText"));
                }
                else
                {
                    string[] options = new string[myTarget.Stats.Count];
                    for (int i = 0; i < options.Length; i++) options[i] = myTarget.stats[i].name;
                    if (selStat > options.Length - 1) selStat = options.Length - 1;
                    selStat = EditorGUILayout.Popup("Stat Value", selStat, options);

                    SectionHeader("Current Values");
                    if (myTarget.Stats != null && myTarget.Stats.Count > 0)
                    {
                        StatValue stat = myTarget.Stats[selStat];
                        EditorGUILayout.LabelField("Value", stat.CurrentValue.ToString());
                        EditorGUILayout.LabelField("Min", stat.CurrentMinimum.ToString());
                        EditorGUILayout.LabelField("Max", stat.CurrentMaximum.ToString());
                        if (stat.enableRegen)
                        {
                            EditorGUILayout.LabelField("Regen", stat.CurrentRegenAmount.ToString());
                            EditorGUILayout.LabelField("Regen Delay", stat.CurrentRegenDelay.ToString());
                        }

                        SectionHeader("Base Values");
                        EditorGUILayout.LabelField("Value", stat.CurrentBaseValue.ToString());
                        EditorGUILayout.LabelField("Min", stat.CurrentBaseMinimum.ToString());
                        EditorGUILayout.LabelField("Max", stat.CurrentBaseMaximum.ToString());
                        if (stat.enableRegen)
                        {
                            EditorGUILayout.LabelField("Regen", stat.CurrentBaseRegenAmount.ToString());
                            EditorGUILayout.LabelField("Regen Delay", stat.CurrentBaseRegenDelay.ToString());
                        }

                        SectionHeader("Active Modifiers");
                        if (stat.ActiveModifiers != null && stat.ActiveModifiers.Count == 0)
                        {
                            EditorGUILayout.LabelField("No modifiers active.", Skin.GetStyle("WrapText"));
                        }
                        else
                        {
                            for (int i = 0; i < stat.ActiveModifiers.Count; i++)
                            {
                                EditorGUILayout.LabelField("[" + i + "]", stat.ActiveModifiers[i].AppliedValue.ToString());
                            }
                        }
                    }
                }
            }

            tmp = showEffectsDebug;
            showEffectsDebug = SectionGroup("Active Effects", showEffectsDebug);
            if (tmp)
            {
                if (myTarget.Effects == null || myTarget.Effects.Count == 0)
                {
                    EditorGUILayout.LabelField("There are no active effects.", Skin.GetStyle("WrapText"));
                }
                else
                {
                    for (int i = 0; i < myTarget.Effects.Count; i++)
                    {
                        EditorGUILayout.LabelField("[" + i + "] " + myTarget.Effects[i].displayName);
                    }
                }
            }

            SectionHeader("Console");
            scroll = EditorGUILayout.BeginScrollView(scroll);
            command = EditorGUILayout.TextArea(command, GUILayout.Height(30));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Process Command"))
            {
                myTarget.SendCommand(command);
                command = string.Empty;
                scroll = Vector2.zero;
                EditorUtility.SetDirty(target);
                Repaint();
            }
        }

        private void DrawDragBox(SerializedProperty values, SerializedProperty effects)
        {
            GUILayout.BeginVertical();
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.normal.textColor = Color.white;

            DragBox("Drag & Drop Values/Effects Here");

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
                            StatEffect effect = dragged as StatEffect;
                            if (effect != null)
                            {
                                if (HasValue(effects, effect))
                                {
                                    EditorUtility.DisplayDialog("Stats Cog", "An effect named '" + effect.name + "' already exists on this StatsCog.", "Ok");
                                }
                                else
                                {
                                    effects.arraySize++;
                                    effects.GetArrayElementAtIndex(effects.arraySize - 1).objectReferenceValue = effect;
                                }
                                continue;
                            }

                            StatValue stat = dragged as StatValue;
                            if (stat != null)
                            {
                                if (HasValue(values, stat))
                                {
                                    EditorUtility.DisplayDialog("Stats Cog", "A value named '" + stat.name + "' already exists on this StatsCog.", "Ok");
                                }
                                else
                                {
                                    values.arraySize++;
                                    values.GetArrayElementAtIndex(values.arraySize - 1).objectReferenceValue = stat;
                                }
                                continue;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawEffects()
        {
            SimpleProperty("effectList");

            SectionHeader("Starting Effects", "startingEffects", typeof(StatEffect));
            startingSP = SimpleList("startingEffects", startingSP, 120, 1);

            SectionHeader("Resistance");
            resistSP = SimpleList("effectResistances", resistSP, 120, 1);
        }

        private void DrawEvents()
        {
            VerticalSpace(6);

            SimpleProperty("onEffectAdded");
            SimpleProperty("onEffectEnded");
            SimpleProperty("onEffectRemoved");
            SimpleProperty("onEffectResisted");

            VerticalSpace(6);

            SimpleProperty("onDamageTaken");
            SimpleProperty("onImmuneToDamage");
            SimpleProperty("onDeath");
        }

        private void DragStatBox(SerializedProperty list)
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
                            var obj = dragged as StatValue;
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

        private bool HasValue(SerializedProperty list, Object value)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                if (list.GetArrayElementAtIndex(i).objectReferenceValue.name == value.name)
                {
                    return true;
                }
            }

            return false;
        }

        private void RemoveElementAtIndex(SerializedProperty array, int index)
        {
            if (index != array.arraySize - 1)
            {
                array.GetArrayElementAtIndex(index).objectReferenceValue = array.GetArrayElementAtIndex(array.arraySize - 1).objectReferenceValue;
            }
            array.arraySize--;
        }

        private void ResistanceList(SerializedProperty list)
        {
            EditorGUILayout.Separator();

            for (int i = 0; i < list.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    RemoveElementAtIndex(list, i);
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
                list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("value").stringValue = "0";
            }

            if (GUILayout.Button("Clear"))
            {
                list.arraySize = 0;
            }
            GUILayout.EndHorizontal();
        }

        private void WeaknessList(SerializedProperty list)
        {
            EditorGUILayout.Separator();

            for (int i = 0; i < list.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    RemoveElementAtIndex(list, i);
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
                list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("value").stringValue = "0";
                list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("maxValue").stringValue = "4";
            }

            if (GUILayout.Button("Clear"))
            {
                list.arraySize = 0;
            }
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Window Methods

        internal void DrawEffectsList()
        {
            SimpleProperty("effectList");
            if (myTarget.effectList == null)
            {
                if (GUILayout.Button("Create New Effect List"))
                {
                    if (string.IsNullOrWhiteSpace(myTarget.effectFolder))
                    {
                        myTarget.effectFolder = Application.dataPath;
                    }
                    string path = EditorUtility.SaveFilePanelInProject("Save Category", "New Effect List", "asset", "Select a location to save the Effect List", myTarget.effectFolder);
                    if (path.Length != 0)
                    {
                        myTarget.effectFolder = System.IO.Path.GetDirectoryName(path);

                        StatEffectList effectList = (StatEffectList)ScriptableObject.CreateInstance(typeof(StatEffectList));
                        effectList.name = System.IO.Path.GetFileNameWithoutExtension(path);
                        AssetDatabase.CreateAsset(effectList, path);
                        AssetDatabase.SaveAssets();

                        myTarget.effectList = effectList;
                    }
                }
            }
        }

        internal int DrawStatsList()
        {
            int result = -1;
            stats.DoLayoutList();
            if (stats.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = stats.index;
                }
            }
            return result;
        }

        #endregion

    }
}