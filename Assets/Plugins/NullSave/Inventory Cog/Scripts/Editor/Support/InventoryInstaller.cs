using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class InventoryInstaller : AssetPostprocessor
    {

        #region Unity Methods

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets)
            {
                if (importedAsset.Contains("InventoryInstaller.cs"))
                {
                    InventoryWelcomeWindow.ShowWindow();
                    Cleanup();
                    InstallDependencies();
                    return;
                }
            }
        }

        public static void InstallDependencies()
        {
            if (!IsInstalled("com.unity.cinemachine"))
            {
                Install("com.unity.cinemachine@2.2.0");
            }
        }

        #endregion

        #region Private Methods

        static void Cleanup()
        {
            string root = FindNullSaveRoot();

            RemoveOldFiles(root);
            //RemoveOldDemos(root);

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

        static void RemoveOldDemos(string root)
        {
            bool wasPrompted = false;

            // Remove old demos
            string path = root + "/[Examples]/Inventory Cog/Cute Crafting Inventory";
            if (Directory.Exists(path))
            {
                if (!wasPrompted)
                {
                    if (!RemoveOldDemosPrompt()) return;
                    wasPrompted = true;
                }
                Directory.Delete(path, true);
                File.Delete(path + ".meta");
            }

            path = root + "/[Examples]/Inventory Cog/Sci-Fi Inventory";
            if (Directory.Exists(path))
            {
                if (!wasPrompted)
                {
                    if (!RemoveOldDemosPrompt()) return;
                    wasPrompted = true;
                }
                Directory.Delete(path, true);
                File.Delete(path + ".meta");
            }

            path = root + "/[Examples]/Inventory Cog/Stylized Adventure Inventory";
            if (Directory.Exists(path))
            {
                if (!wasPrompted)
                {
                    if (!RemoveOldDemosPrompt()) return;
                    wasPrompted = true;
                }
                Directory.Delete(path, true);
                File.Delete(path + ".meta");
            }

            path = root + "/[Examples]/Inventory Cog/Shared";
            if (Directory.Exists(path))
            {
                if (!wasPrompted)
                {
                    if (!RemoveOldDemosPrompt()) return;
                    wasPrompted = true;
                }
                Directory.Delete(path, true);
                File.Delete(path + ".meta");
            }

        }

        static bool RemoveOldDemosPrompt()
        {
            return EditorUtility.DisplayDialog("Inventory Cog", "Obsolete demos have been detected. Would you like to remove them now?", "Yes", "No");
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