using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace NullSave
{
#if HICONS
    [InitializeOnLoad]
    public class HierarchyIcons
    {

    #region Variables

        static Texture2D texturePanel;
        static List<int> markedObjects;
        static Dictionary<int, HierarchyData> clean;
        static Texture2D childIcon, moreIcon;

    #endregion

    #region Constructor

        static HierarchyIcons()
        {
            clean = new Dictionary<int, HierarchyData>();
            EditorApplication.hierarchyWindowItemOnGUI += AddIcons;
            EditorApplication.hierarchyChanged += () =>
            {
                Clear();
            };
            EditorApplication.projectChanged += () =>
            {
                Clear();
            };
            Application.focusChanged += FocusChanged();
        }

        [MenuItem("Tools/NullSave/Disable Hierarchy Icons", false)]
        public static void DisableIcons()
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            defines = defines.Replace(";HICONS", "").Replace("HICONS", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }

        #endregion

        #region Private Methods

        private static void AddIcons(int instanceId, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (go == null) return;

            if (clean.ContainsKey(instanceId))
            {
                HierarchyData data = clean[instanceId];
                if (data.selectionRect == selectionRect)
                {
                    data = clean[instanceId];
                    for (int i = 0; i < data.colors.Count; i++)
                    {
                        GUI.DrawTexture(data.rects[i], data.icons[i], ScaleMode.ScaleToFit, true, 0, data.colors[i], 0, 0);
                    }
                    return;
                }
                else
                {
                    clean.Remove(instanceId);
                }
            }

            RedrawIcons(instanceId, selectionRect, go);
        }

        private static void Clear()
        {
#if UNITY_EDITOR
            if (EditorGUIUtility.isProSkin)
            {
                childIcon = GetTex("child_pro");
            }
            else
            {
                childIcon = GetTex("child");
            }
#endif
            if(moreIcon == null) moreIcon = GetTex("more_icons");
            clean.Clear();
        }

        private static System.Action<bool> FocusChanged()
        {
            Clear();
            return null;
        }

        private static bool NeedsMoreIcon(MonoBehaviour[] behaviours, GameObject go)
        {
            if (behaviours.Length < 6) return false;

            int count = 0;
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i].gameObject == go)
                {
                    count += 1;
                    if (count >= 6) return true;
                }
                else
                {
                    HierarchyIconAttribute icon = behaviours[i].GetType().GetCustomAttribute(typeof(HierarchyIconAttribute)) as HierarchyIconAttribute;
                    if (icon != null && icon.BubbleUp)
                    {
                        count += 1;
                        if (count >= 6) return true;
                    }
                }
            }

            return false;
        }

        private static Texture2D GetTex(string name)
        {
            return (Texture2D)Resources.Load("Icons/" + name);
        }

        private static void RedrawIcons(int instanceId, Rect selectionRect, GameObject go)
        {
            float offset = 18;
            MonoBehaviour[] behaviours = go.GetComponentsInChildren<MonoBehaviour>();
            HierarchyData data = new HierarchyData();
            List<HierarchyIconAttribute> icons = new List<HierarchyIconAttribute>();
            List<HierarchyIconAttribute> childIcons = new List<HierarchyIconAttribute>();
            HierarchyIconAttribute icon;

            // Get top-level and child icons
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour != null)
                {
                    icon = behaviour.GetType().GetCustomAttribute(typeof(HierarchyIconAttribute)) as HierarchyIconAttribute;
                    if (icon != null)
                    {
                        if (behaviour.gameObject == go)
                        {
                            icons.Add(icon);
                        }
                        else if (icon.BubbleUp)
                        {
                            childIcons.Add(icon);
                        }
                    }
                }
            }

            // Check if "More" icon is needed
            if (icons.Count + childIcons.Count > 5)
            {
                Rect r = new Rect(selectionRect.x + selectionRect.width - offset, selectionRect.y, 16f, 16f);
                GUI.DrawTexture(r, moreIcon, ScaleMode.ScaleToFit, true, 0, Color.white, 0, 0);
                offset += 18;

                data.icons.Add(moreIcon);
                data.colors.Add(Color.white);
                data.rects.Add(r);
            }

            // Draw child icons
            int availSpots = 5 - icons.Count;
            while (availSpots > 0 && childIcons.Count > 0)
            {
                icon = childIcons[childIcons.Count - 1];
                Texture2D t = GetTex(icon.IconName);

                Color c = icon.IconColor;

                Rect r = new Rect(selectionRect.x + selectionRect.width - offset, selectionRect.y, 16f, 16f);
                GUI.DrawTexture(r, t, ScaleMode.ScaleToFit, true, 0, c, 0, 0);
                data.icons.Add(t);
                data.colors.Add(c);
                data.rects.Add(r);

                // Add child icon
                GUI.DrawTexture(r, childIcon, ScaleMode.ScaleToFit, true, 0, Color.white, 0, 0);

                data.icons.Add(t);
                data.colors.Add(c);
                data.rects.Add(r);

                offset += 18;

                childIcons.Remove(icon);
                availSpots -= 1;
            }

            // Draw top-level icons
            availSpots = 5;
            while (availSpots > 0 && icons.Count > 0)
            {
                icon = icons[icons.Count - 1];
                Texture2D t = GetTex(icon.IconName);

                Color c = icon.IconColor;

                Rect r = new Rect(selectionRect.x + selectionRect.width - offset, selectionRect.y, 16f, 16f);
                GUI.DrawTexture(r, t, ScaleMode.ScaleToFit, true, 0, c, 0, 0);
                data.icons.Add(t);
                data.colors.Add(c);
                data.rects.Add(r);

                offset += 18;

                icons.Remove(icon);
                availSpots -= 1;
            }

            clean.Add(instanceId, data);
        }

    #endregion

    }
#else

    public class HierarchyIcons
    {

        [MenuItem("Tools/NullSave/Enable Hierarchy Icons", false)]
        public static void EnableIcons()
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!string.IsNullOrWhiteSpace(defines)) defines += ";";
            defines += "HICONS";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }

    }

#endif
}

