using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class InventoryDLCUpdater : MonoBehaviour
    {

        #region Variables

        public GenericPlatform targetPlatform = GenericPlatform.Desktop;
        public string host;
        public List<string> bundles;
        public bool includeManifestFiles = true;
        public int bufferKb = 100;

        public Slider progress;

        public DLCUpdate onDLCUpdate;
        public UnityEvent onDLCComplete;

        #endregion

        #region Unity Methods

        private void Start()
        {
            if (!MatchesPlatform()) return;
            StartCoroutine("DownloadBundles");
            if (progress != null)
            {
                onDLCUpdate.AddListener(UpdateUI);
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator DownloadBundles()
        {

            // Setup file list
            List<string> fileList = new List<string>();
            foreach (string bundle in bundles)
            {
                if (includeManifestFiles) fileList.Add(host + bundle + ".manifest");
                fileList.Add(host + bundle);
            }

            // Get Files
            int fileIndex = 1;
            string path = Application.streamingAssetsPath;
            foreach (string file in fileList)
            {
                using (var client = new WebClient())
                {
                    string filename = Path.Combine(path, Path.GetFileName(file));

                    System.DateTime localLastMod = File.Exists(filename) ? new FileInfo(filename).LastWriteTime : new System.DateTime(1980, 1, 1);

                    // Get bundle
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(file);
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.LastModified <= localLastMod)
                        {
                            onDLCUpdate?.Invoke(fileIndex, fileList.Count, 1, 1);
                            yield return null;
                        }
                        else
                        {
                            byte[] buffer = new byte[bufferKb * 1024];
                            int read;
                            long totalRead = 0;
                            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                using (Stream stream = response.GetResponseStream())
                                {
                                    while (totalRead < response.ContentLength)
                                    {
                                        read = stream.Read(buffer, 0, buffer.Length);
                                        totalRead += read;
                                        fs.Write(buffer, 0, read);
                                        onDLCUpdate?.Invoke(fileIndex, fileList.Count, totalRead, response.ContentLength);
                                        yield return null;
                                    }
                                }
                            }

                            // Load bundle
                            if (!filename.EndsWith(".manifest"))
                            {
                                try
                                {
                                    AssetBundle loadedBundle = AssetBundle.LoadFromFile(filename);
                                    if (loadedBundle != null)
                                    {
                                        string[] assets = loadedBundle.GetAllAssetNames();
                                        foreach (string asset in assets)
                                        {
                                            Object obj = loadedBundle.LoadAsset(asset);
                                        }
                                    }
                                }
                                catch (System.Exception) { }
                            }

                        }
                    }


                    fileIndex += 1;
                }
            }

            InventoryDB.RefreshDatabase();
            onDLCComplete?.Invoke();
        }

        private bool MatchesPlatform()
        {
            switch (targetPlatform)
            {
                case GenericPlatform.Desktop:
                    if (Application.platform == RuntimePlatform.LinuxEditor ||
                    Application.platform == RuntimePlatform.LinuxPlayer ||
                        Application.platform == RuntimePlatform.OSXEditor ||
                        Application.platform == RuntimePlatform.OSXPlayer) return true;
                    if (SystemInfo.deviceModel == "Xbox One (Microsoft)") return false;
                    if (SystemInfo.operatingSystem == "Console") return false;
                    switch (Application.platform)
                    {
                        case RuntimePlatform.WindowsEditor:
                        case RuntimePlatform.WindowsPlayer:
                        case RuntimePlatform.WSAPlayerARM:
                        case RuntimePlatform.WSAPlayerX64:
                        case RuntimePlatform.WSAPlayerX86:
                            if (SystemInfo.deviceModel == "Xbox One (Microsoft)") return false;
                            if (SystemInfo.operatingSystem == "Console") return false;
                            return true;
                        default:
                            return false;
                    }
                case GenericPlatform.Mobile:
                    return (Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.platform == RuntimePlatform.Android);
                case GenericPlatform.Android:
                    return Application.platform == RuntimePlatform.Android;
                case GenericPlatform.IPhone:
                    return Application.platform == RuntimePlatform.IPhonePlayer;
                case GenericPlatform.Linux:
                    return (Application.platform == RuntimePlatform.LinuxEditor ||
                    Application.platform == RuntimePlatform.LinuxPlayer);
                case GenericPlatform.OSX:
                    return (Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer);
                case GenericPlatform.PS4:
                    return Application.platform == RuntimePlatform.PS4;
                case GenericPlatform.Switch:
                    return Application.platform == RuntimePlatform.Switch;
                case GenericPlatform.Windows:
                    if (SystemInfo.deviceModel == "Xbox One (Microsoft)") return false;
                    if (SystemInfo.operatingSystem == "Console") return false;
                    switch (Application.platform)
                    {
                        case RuntimePlatform.WindowsEditor:
                        case RuntimePlatform.WindowsPlayer:
                        case RuntimePlatform.WSAPlayerARM:
                        case RuntimePlatform.WSAPlayerX64:
                        case RuntimePlatform.WSAPlayerX86:
                            if (SystemInfo.deviceModel == "Xbox One (Microsoft)") return false;
                            if (SystemInfo.operatingSystem == "Console") return false;
                            return true;
                        default:
                            return false;
                    }
                case GenericPlatform.XBOXOne:
                    if (SystemInfo.deviceModel == "Xbox One (Microsoft)") return true;
                    if (SystemInfo.operatingSystem == "Console") return true;
                    return Application.platform == RuntimePlatform.XboxOne;
                default:
                    return false;
            }
        }

        private void UpdateUI(int curFile, int totalFiles, long fileDownloaded, long fileLength)
        {
            progress.maxValue = totalFiles;
            progress.minValue = 0;
            progress.value = curFile + ((float) fileDownloaded / (float)fileLength);
        }

        #endregion

    }
}