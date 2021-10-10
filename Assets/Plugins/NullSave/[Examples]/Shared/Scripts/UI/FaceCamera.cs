using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class FaceCamera : MonoBehaviour
    {

        #region Variables



        #endregion

        #region Unity Methods

        private void Update()
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }

        #endregion

    }
}