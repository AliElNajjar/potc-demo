using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(CraftingRecipe))]
    public class CraftingRecipeEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            UI = 1,
            Behaviour = 2,
        }

        #endregion

        #region Variables

        private DisplayFlags displayFlags;
        private Texture2D behaviourIcon, uiIcon;
        private Vector2 componentsSP, resultSP, failedSP, advComponentsSP;

        #endregion

        #region Properties

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

        #endregion

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBegin("Crafting Recipe", "Icons/tock-book", false);

            DrawUI();

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void DrawUI()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.UI, "UI", UIIcon))
            {
                SimpleProperty("icon");
                SimpleProperty("displayName");
                SimpleProperty("description");
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Behaviour, "Behaviour", BehaviourIcon))
            {
                SimpleProperty("craftingCategory");
                StatsBoolToggle("unlocked", "Unlocked", "unlockedSource", "unlockedExpression");
                StatsBoolToggle("displayInList", "Display In List", "displaySource", "displayExpression");
                SimpleProperty("rarity");

                SimpleProperty("valueSource", "Value Source");
                if ((ValueSource)serializedObject.FindProperty("valueSource").intValue == ValueSource.Static)
                {
                    SimpleProperty("value");
                }
                else
                {
                    SimpleProperty("valueStatName", "Value");
                }

                SimpleProperty("craftType");
                SimpleProperty("craftTime");
                if ((CraftingTime)serializedObject.FindProperty("craftTime").intValue != CraftingTime.Instant)
                {
                    SimpleProperty("craftSeconds");
                }
                SimpleProperty("successSource", "Success Source");
                if ((ValueSource)serializedObject.FindProperty("successSource").intValue == ValueSource.Static)
                {
                    SimpleProperty("successChance");
                }
                else
                {
                    SimpleProperty("successStatName", "Success Chance");
                }
            }

            if ((CraftingType)serializedObject.FindProperty("craftType").intValue == CraftingType.Create)
            {
                SectionHeader("Components");
                SimpleProperty("componentType");
                if ((ComponentType)serializedObject.FindProperty("componentType").intValue == ComponentType.Standard)
                {
                    componentsSP = SimpleList("components", componentsSP, 150, 2);
                }
                else
                {
                    advComponentsSP = SimpleList("advancedComponents", advComponentsSP, 150, 2);
                }

                SectionHeader("Success Result");
                resultSP = SimpleList("result", resultSP, 182, 5);

                SectionHeader("Failed Result");
                failedSP = SimpleList("failResult", failedSP, 182, 5);
            }
            else
            {
                SectionHeader("Base Item");
                GUILayout.BeginHorizontal();
                SimpleProperty("baseItem", null);
                GUILayout.EndHorizontal();

                SectionHeader("Additional Components");
                SimpleProperty("componentType");
                if ((ComponentType)serializedObject.FindProperty("componentType").intValue == ComponentType.Standard)
                {
                    componentsSP = SimpleList("components", componentsSP, 150, 2);
                }
                else
                {
                    advComponentsSP = SimpleList("advancedComponents", advComponentsSP, 150, 2);
                }

                SectionHeader("Success Result");
                GUILayout.BeginHorizontal();
                SimpleProperty("upgradeSuccess", null);
                GUILayout.EndHorizontal();

                SectionHeader("Failed Upgrade");
                SimpleProperty("destoryOnFail");
            }
        }

        private void StatsBoolToggle(string baseValue, string displayName, string sourceValue, string expressionValue)
        {
            if ((BooleanSource)serializedObject.FindProperty(sourceValue).intValue == BooleanSource.Static)
            {
                EditorGUILayout.BeginHorizontal();
                SimpleProperty(sourceValue, displayName);
                serializedObject.FindProperty(baseValue).boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty(baseValue).boolValue);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginVertical();
                SimpleProperty(sourceValue, displayName);
                SimpleProperty(expressionValue, " ");
                EditorGUILayout.EndVertical();
            }
        }

        #endregion

        #region Window Methods

        public void DrawInspector()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            DrawUI();
        }

        #endregion

    }
}