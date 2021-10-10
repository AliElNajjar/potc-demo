using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(InventoryDLCUpdater))]
    public class InventoryDLCUpdaterEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Inventory DLC", "Icons/tock-event");

            SectionHeader("Behaviour");
            SimpleProperty("targetPlatform");
            SimpleProperty("host");
            GUILayout.Label("Bundle Files");
            SimpleList("bundles");
            SimpleProperty("includeManifestFiles");
            SimpleProperty("bufferKb");

            SectionHeader("UI");
            SimpleProperty("progress");

            SectionHeader("Events");
            SimpleProperty("onDLCUpdate");
            SimpleProperty("onDLCComplete");

            MainContainerEnd();
        }

        #endregion

    }
}
