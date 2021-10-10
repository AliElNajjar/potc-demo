using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK
{
    [HierarchyIcon("tock-key-trigger", false)]
    public class TriggerByButton : MonoBehaviour
    {

        #region Variables

        public string buttonName = "Submit";
        public UnityEvent onButton, onButtonDown, onButtonUp;

        #endregion

        #region Unity Methods

        private void Update()
        {
#if GAME_COG
            if (GameCog.Input.GetButton(buttonName))
            {
                onButton?.Invoke();
            }
            if (GameCog.Input.GetButtonDown(buttonName))
            {
                onButtonDown?.Invoke();
            }
            if (GameCog.Input.GetButtonUp(buttonName))
            {
                onButtonUp.Invoke();
            }
#else
            if (Input.GetButton(buttonName))
            {
                onButton?.Invoke();
            }
            if (Input.GetButtonDown(buttonName))
            {
                onButtonDown?.Invoke();
            }
            if (Input.GetButtonUp(buttonName))
            {
                onButtonUp.Invoke();
            }
#endif
        }

        #endregion

    }
}