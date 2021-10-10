using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK
{
    public class ActionRaycastTarget : MonoBehaviour
    {

        #region Variables

        public string displayText;
        public bool interactable = true;

        public ActionMenu actionMenu;
        public MenuOpenType actionOpen = MenuOpenType.ActiveGameObject;
        public Transform menuContainer;
        public string spawnTag;

        public UnityEvent onActionTriggered;

        #endregion

        #region Variables

        public ActionRaycastTrigger Caster { get; set; }

        #endregion

        #region Public Methods

        public void ActivateTrigger()
        {
            if (Caster == null) return;

            if (actionMenu != null)
            {
                Caster.actionMenu = actionMenu;
                Caster.actionOpen = actionOpen;
                Caster.menuContainer = menuContainer;
                Caster.spawnTag = spawnTag;
                Caster.OpenMenu();
            }

            onActionTriggered?.Invoke();
        }

        #endregion

    }
}