using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.DLC
{
    [CustomEditor(typeof(DLCBundler))]
    public class DLCBundlerEditor : TOCKEditorV2
    {

        #region Variables

        private DLCBundler myTarget;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            myTarget = (DLCBundler)target;
        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin("DLC Bundler", "Icons/tock-repair");

            SimpleProperty("bundleOutput");

            string[] options = new string[] { "UncompressedAssetBundle", "UncompressedAssetBundle", "CollectDependencies", "CompleteAssets", "DisableWriteTypeTree",
                "DeterministicAssetBundle", "ForceRebuildAssetBundle", "IgnoreTypeTreeChanges", "AppendHashToAssetBundleName", "ChunkBasedCompression", "StrictMode",
                "DryRunBuild", "DisableLoadAssetByFileName", "DisableLoadAssetByFileNameWithExtension" };
            int flags = serializedObject.FindProperty("bundleOptions").intValue;
            serializedObject.FindProperty("bundleOptions").intValue = EditorGUILayout.MaskField("Bundle Options", flags, options);

            SimpleProperty("buildTarget");

            if (GUILayout.Button("Build Bundle"))
            {
                string assetBundleDirectory = serializedObject.FindProperty("bundleOutput").stringValue;
                if (!Directory.Exists(assetBundleDirectory))
                {
                    Directory.CreateDirectory(assetBundleDirectory);
                }
                BuildPipeline.BuildAssetBundles(assetBundleDirectory, (BuildAssetBundleOptions)serializedObject.FindProperty("bundleOptions").intValue,
                    (BuildTarget)serializedObject.FindProperty("buildTarget").intValue);
            }

            MainContainerEnd();
        }

        #endregion

    }
}