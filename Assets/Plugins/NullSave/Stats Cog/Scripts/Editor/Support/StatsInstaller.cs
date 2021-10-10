using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatsInstaller : AssetPostprocessor
    {

        #region Unity Methods

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets)
            {
                if (importedAsset.Contains("StatsInstaller.cs"))
                {
                    StatsWelcomeWindow.ShowWindow();
                    Cleanup();
                    InstallDependencies();
                    return;
                }
            }
        }

        public static void InstallDependencies()
        {
            //if (!IsInstalled("com.unity.cinemachine"))
            //{
            //    Install("com.unity.cinemachine@2.2.9");
            //}
            //if (!IsInstalled("com.unity.postprocessing"))
            //{
            //    Install("com.unity.postprocessing");
            //}
        }

        #endregion

        #region Private Methods

        static void Cleanup()
        {
            string root = FindNullSaveRoot();

            RemoveOldFiles(root);

            AssetDatabase.Refresh();
        }

        static string FindNullSaveRoot()
        {
            string[] dirs = Directory.GetDirectories(Application.dataPath, "NullSave", SearchOption.AllDirectories);
            if (dirs != null && dirs.Length > 0)
            {
                return dirs[0].Replace("\\", "/");
            }

            return Application.dataPath;
        }

        static bool IsInstalled(string packageID)
        {
            string packagesFolder = Application.dataPath + "/../Packages/";
            string manifestFile = packagesFolder + "manifest.json";
            string manifest = File.ReadAllText(manifestFile);
            return manifest.Contains(packageID);
        }

        static void Install(string packageVersionID)
        {
            Client.Add(packageVersionID);
        }

        static void RemoveOldFiles(string root)
        {
            // Remove old data store
            string path = root + "/Shared/Scripts/Support/DataStore.cs";
            if (File.Exists(path))
            {
                File.Delete(path);
                File.Delete(path + ".meta");
            }
        }

        #endregion

    }
}