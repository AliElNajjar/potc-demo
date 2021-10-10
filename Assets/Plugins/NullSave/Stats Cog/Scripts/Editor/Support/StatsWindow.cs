using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatsWindow : TOCKEditorWindow
    {

        #region Variables

        public StatsCog stats;
        private StatsCogEditor statsEditor;
        private bool warnPrefab;
        private GameObject prefabRoot;
        private int selToolbarIndex;

        // Stats
        private Vector2 statsPos, statPos;
        private StatValue addStat, editStat;
        private StatValueEditor statEditor;

        // Effects
        private Vector2 effectsPos, effectPos;
        private StatEffectList lastEffects;
        private StatEffectListEditor effectListEditor;
        private StatEffect addEffect, editEffect;
        private StatEffectEditor effectEditor;


        private static Texture2D statsIcon;
        private static GUIStyle warningLabel;
        private static readonly string[] toolbarOptions = new string[] { "Stats", "Effects", "Combat" };

        #endregion

        #region Properties

        private static Texture2D StatsIcon
        {
            get
            {
                if (statsIcon == null)
                {
                    statsIcon = (Texture2D)Resources.Load("Icons/effects-icon", typeof(Texture2D));
                }

                return statsIcon;
            }
        }

        private static GUIStyle WarningLabel
        {
            get
            {
                if (warningLabel == null)
                {
                    warningLabel = new GUIStyle(EditorStyles.label);
                    warningLabel.normal.textColor = Color.yellow;
                }

                return warningLabel;
            }
        }

        #endregion

        #region Unity Methods

        private void OnGUI()
        {
            ContainerBegin();
            if (HaveStats())
            {
                if (warnPrefab)
                {
                    DrawPrefabWarning();
                }

                if (statsEditor != null)
                {
                    statsEditor.serializedObject.Update();
                }
                if (effectListEditor != null)
                {
                    effectListEditor.serializedObject.Update();
                }
                DrawButtons();

                switch (selToolbarIndex)
                {
                    case 0:
                        DrawStats();
                        break;
                    case 1:
                        DrawEffects();
                        break;
                    case 2:
                        GUILayout.BeginVertical(GUILayout.Width(300));
                        statsEditor.DrawCombat();
                        GUILayout.EndVertical();
                        break;
                }

                if (statsEditor != null)
                {
                    statsEditor.serializedObject.ApplyModifiedProperties();
                }
                if (effectListEditor != null)
                {
                    effectListEditor.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                GUILayout.Label("Please select a Stats Cog instance in the scene.");
            }
            ContainerEnd();

        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        #endregion

        #region Public Methods

        public static void Open()
        {
            StatsWindow w = GetWindow<StatsWindow>("Stats & Effects Editor", typeof(SceneView));
            w.titleContent = new GUIContent("Stats & Effects Editor", StatsIcon);
            w.wantsMouseMove = true;
        }

        #endregion

        #region Private Methods

        private void ContainerBegin()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
        }

        private void ContainerEnd()
        {
            GUILayout.EndVertical();
            GUILayout.Space(8);
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
            GUILayout.EndVertical();
        }

        private void DrawButtons()
        {
            GUILayout.BeginVertical();
            selToolbarIndex = GUILayout.Toolbar(selToolbarIndex, toolbarOptions, GUILayout.Width(600));
            GUILayout.EndVertical();
        }

        private void DrawEffects()
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(750));

            GUILayout.BeginVertical(GUILayout.Width(300));
            effectsPos = GUILayout.BeginScrollView(effectsPos);
            statsEditor.DrawEffectsList();

            if (stats.effectList != lastEffects)
            {
                lastEffects = stats.effectList;
                if (lastEffects != null)
                {
                    effectListEditor = (StatEffectListEditor)Editor.CreateEditor(stats.effectList, typeof(StatEffectListEditor));
                }
            }

            if (effectListEditor == null) return;

            int editId = effectListEditor.DrawEffectList();
            if (editId > -1)
            {
                editEffect = lastEffects.availableEffects[editId];
                effectEditor = (StatEffectEditor)Editor.CreateEditor(editEffect);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(32);

            GUILayout.BeginVertical(GUILayout.Width(300));
            if (editEffect == null)
            {
                if (addEffect == null)
                {
                    addEffect = (StatEffect)ScriptableObject.CreateInstance(typeof(StatEffect));
                    addEffect.name = "New Effect";
                    effectEditor = (StatEffectEditor)Editor.CreateEditor(addEffect);
                }

                effectEditor.serializedObject.Update();
                SectionHeader("Create Effect");
                effectPos = GUILayout.BeginScrollView(effectPos);
                effectEditor.DrawInspector();
                GUILayout.EndScrollView();

                GUILayout.Space(16);
                if (GUILayout.Button("Create and Add"))
                {
                    if (string.IsNullOrWhiteSpace(stats.effectFolder))
                    {
                        stats.effectFolder = Application.dataPath;
                    }
                    string path = EditorUtility.SaveFilePanelInProject("Save Effect", addEffect.displayName, "asset", "Select a location to save the effect", stats.effectFolder);
                    if (path.Length != 0)
                    {
                        effectListEditor.serializedObject.Update();
                        stats.effectFolder = System.IO.Path.GetDirectoryName(path);

                        addEffect.name = System.IO.Path.GetFileNameWithoutExtension(path);
                        AssetDatabase.CreateAsset(addEffect, path);
                        AssetDatabase.SaveAssets();

                        effectListEditor.AddEffect(AssetDatabase.LoadAssetAtPath(path, typeof(StatEffect)) as StatEffect);
                        addEffect = null;
                        effectPos = Vector2.zero;
                        GUI.FocusControl("Clear");
                        effectListEditor.serializedObject.ApplyModifiedProperties();
                    }
                }

                effectEditor.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                effectEditor.serializedObject.Update();
                SectionHeader("Edit Effect");
                effectPos = GUILayout.BeginScrollView(effectPos);
                effectEditor.DrawInspector();
                GUILayout.EndScrollView();

                GUILayout.Space(16);
                if (GUILayout.Button("Done"))
                {
                    addEffect = null;
                    editEffect = null;
                    GUI.FocusControl("Clear");
                    Repaint();

                }
                else
                {
                    effectEditor.serializedObject.ApplyModifiedProperties();
                }
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawPrefabWarning()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            //if (GUILayout.Button("Edit Prefab", GUILayout.Width(100)))
            //{
            //    Selection.activeObject = AssetDatabase.LoadAssetAtPath(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot), typeof(Object));
            //    AssetDatabase.OpenAsset(Selection.activeObject);
            //    stats = ((GameObject)Selection.activeObject).GetComponent<Inventorystats>();
            //}
            //GUILayout.Label("You are editing a prefab instance, this will not save to directly to the prefab.", WarningLabel);
            GUILayout.Label("This version of the editor does not work well with prefabs. Consider using a single instance or unpacking and replacing the prefab when you're done editing.", WarningLabel);

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        private void DrawStats()
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(750));

            GUILayout.BeginVertical(GUILayout.Width(300));
            SectionHeader("Local Stat Values");
            statsPos = GUILayout.BeginScrollView(statsPos);
            int editId = statsEditor.DrawStatsList();
            if (editId > -1)
            {
                editStat = stats.stats[editId];
                statEditor = (StatValueEditor)Editor.CreateEditor(editStat);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(32);

            GUILayout.BeginVertical(GUILayout.Width(300));
            if (editStat == null)
            {
                if (addStat == null)
                {
                    addStat = (StatValue)ScriptableObject.CreateInstance(typeof(StatValue));
                    addStat.name = "New Stat";
                    statEditor = (StatValueEditor)Editor.CreateEditor(addStat);
                }

                statEditor.serializedObject.Update();
                SectionHeader("Create Stat");
                statPos = GUILayout.BeginScrollView(statPos);
                statEditor.DrawInspector();

                GUILayout.Space(16);
                if (GUILayout.Button("Create and Add"))
                {
                    if (string.IsNullOrWhiteSpace(stats.statFolder))
                    {
                        stats.statFolder = Application.dataPath;
                    }
                    string path = EditorUtility.SaveFilePanelInProject("Save Stat", addStat.displayName, "asset", "Select a location to save the stat", stats.statFolder);
                    if (path.Length != 0)
                    {
                        stats.statFolder = System.IO.Path.GetDirectoryName(path);

                        addStat.name = System.IO.Path.GetFileNameWithoutExtension(path);
                        AssetDatabase.CreateAsset(addStat, path);
                        AssetDatabase.SaveAssets();

                        if (stats.stats == null)
                        {
                            stats.stats = new System.Collections.Generic.List<StatValue>();
                        }
                        stats.stats.Add(AssetDatabase.LoadAssetAtPath(path, typeof(StatValue)) as StatValue);
                        addStat = null;
                        statPos = Vector2.zero;
                        GUI.FocusControl("Clear");
                    }
                }
                GUILayout.EndScrollView();

                statEditor.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                statEditor.serializedObject.Update();
                SectionHeader("Edit Stat");
                statPos = GUILayout.BeginScrollView(statPos);
                statEditor.DrawInspector();

                GUILayout.Space(16);
                if (GUILayout.Button("Done"))
                {
                    addStat = null;
                    editStat = null;
                    GUI.FocusControl("Clear");
                    Repaint();

                }
                else
                {
                    statEditor.serializedObject.ApplyModifiedProperties();
                }
                GUILayout.EndScrollView();

            }
            GUILayout.EndVertical();


            GUILayout.EndHorizontal();
        }

        private bool HaveStats()
        {
            GameObject activeObj = Selection.activeObject as GameObject;

            if (activeObj == null)
            {
                stats = null;
                return false;
            }

            prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(activeObj);

            warnPrefab = prefabRoot != null;

            StatsCog newStats = activeObj.GetComponent<StatsCog>();

            if (stats != newStats || (stats == null && newStats != null))
            {
                stats = newStats;
                if (stats == null)
                {
                    statsEditor = null;
                    lastEffects = null;
                    effectListEditor = null;
                    return false;
                }
                else
                {
                    statsEditor = (StatsCogEditor)Editor.CreateEditor(stats, typeof(StatsCogEditor));
                    return true;
                }
            }

            return stats != null;
        }

        #endregion

    }
}