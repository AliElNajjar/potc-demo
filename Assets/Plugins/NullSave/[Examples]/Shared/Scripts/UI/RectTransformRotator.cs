using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformRotator : MonoBehaviour
    {

        #region Variables

        public Vector3 rotationSpeed;

        private RectTransform rt;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        private void Update()
        {
            rt.rotation = Quaternion.Euler(rt.rotation.eulerAngles + rotationSpeed * Time.deltaTime);
        }

        #endregion

    }
}