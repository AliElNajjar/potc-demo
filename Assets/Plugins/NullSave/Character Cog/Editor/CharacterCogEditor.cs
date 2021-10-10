using UnityEditor;

namespace NullSave.TOCK.Character
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(CharacterCog))]
    public class CharacterCogEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Character Cog", "Icons/retro-character");

            SectionHeader("Input");
            SimpleProperty("inputHorizontal", "Horizontal");
            SimpleProperty("inputVertical", "Vertical");
            SimpleProperty("cameraTranslation");
            SimpleProperty("minInputValue");
            SimpleProperty("useMaxInput", "Lock to Max");

            SectionHeader("Plugins", "plugins", typeof(CharacterCogPlugin));
            SimpleList("plugins");

            MainContainerEnd();
        }

        #endregion

    }
}
