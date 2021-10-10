using UnityEngine;

namespace NullSave.TOCK.DLC
{
    public class DLCBundler : MonoBehaviour
    {

        #region Variables

        public string bundleOutput = "Assets/AssetBundles";
#if UNITY_EDITOR
        public UnityEditor.BuildAssetBundleOptions bundleOptions;
        public UnityEditor.BuildTarget buildTarget;
#endif

        #endregion

    }
}