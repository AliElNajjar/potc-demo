using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(RarityColorIndicator))]
    public class RarityColorIndicatorEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Rarity Color Indicator", "Icons/tock-ui");

            //SimpleProperty("rarityColors");
            SerializedProperty list = serializedObject.FindProperty("rarityColors");
            for (int i = 0; i < list.arraySize; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Rarity " + i);
                GUILayout.FlexibleSpace();
                if (i < list.arraySize && i >= 0)
                {
                    
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Empty, null, string.Empty));
                }

                GUILayout.EndHorizontal();
            }


            MainContainerEnd();
        }

        #endregion

    }
}