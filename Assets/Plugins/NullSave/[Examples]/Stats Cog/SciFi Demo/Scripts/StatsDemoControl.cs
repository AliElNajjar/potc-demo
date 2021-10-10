using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatsDemoControl : MonoBehaviour
    {

        #region Variables

        public GameObject menu;

        private bool inMenu;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleMenu();
            }
        }

        #endregion

        #region Private methods

        private void ToggleMenu()
        {
            inMenu = !inMenu;
            menu.SetActive(inMenu);
            Time.timeScale = inMenu ? 0 : 1;
            Cursor.lockState = inMenu ? CursorLockMode.None : CursorLockMode.Locked;

        }

        #endregion

    }
}