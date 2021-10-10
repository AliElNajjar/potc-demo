using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(ItemMenuClient))]
    public class ItemMenuClientEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Menu Client", "Icons/tock-menu");

            SimpleProperty("itemSource");
            SimpleProperty("menuContainer");
            SimpleProperty("useCallingPosition");
            SimpleList("menus");

            MainContainerEnd();
        }

        #endregion

    }
}
