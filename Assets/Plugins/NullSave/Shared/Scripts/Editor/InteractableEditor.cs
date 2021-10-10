using UnityEditor;

namespace NullSave.TOCK
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(Interactable))]
    public class InteractableEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("displayText");
            SimpleProperty("directional");
            if (SimpleBool("directional"))
            {
                SimpleProperty("directionTolerance");

                SectionHeader("Events");
                SimpleProperty("onInteractFront");
                SimpleProperty("onInteractBack");
                SimpleProperty("onInteractLeft");
                SimpleProperty("onInteractRight");
            }
            else
            {
                SectionHeader("Events");
                SimpleProperty("onInteract");
            }

            MainContainerEnd();
        }

        #endregion

    }
}