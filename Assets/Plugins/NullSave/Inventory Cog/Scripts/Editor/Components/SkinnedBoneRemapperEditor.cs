using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(SkinnedBoneRemapper))]
    public class SkinnedBoneRemapperEditor : TOCKEditorV2
    {

        #region Variables

        private ReorderableList boneList;

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            boneList = new ReorderableList(serializedObject, serializedObject.FindProperty("boneNames"), true, true, true, true);
            boneList.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            boneList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Bone Names"); };
            boneList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = boneList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
            };
        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Skinned Bone Remapper", "Icons/tock-map");

            SectionHeader("Behaviour");
            SimpleProperty("rootBone");
            boneList.DoLayoutList();

            GUILayout.Space(8);
            if (GUILayout.Button("Automap"))
            {
                SerializedProperty list = serializedObject.FindProperty("boneNames");
                SkinnedMeshRenderer smr = ((SkinnedBoneRemapper)target).gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    foreach (Transform bone in smr.bones)
                    {
                        list.arraySize++;
                        list.GetArrayElementAtIndex(list.arraySize - 1).stringValue = bone.name;
                    }

                    serializedObject.FindProperty("rootBone").stringValue = smr.rootBone.name;
                }
            }
            if (GUILayout.Button("Clear"))
            {
                SerializedProperty list = serializedObject.FindProperty("boneNames");
                list.ClearArray();
            }

            MainContainerEnd();
        }

        #endregion

    }
}