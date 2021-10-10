using UnityEngine;

namespace NullSave.TOCK.Inventory.Demo
{
    public class TransformRotate : MonoBehaviour
    {

        #region Variables

        public Vector3 rotationSpeed;

        #endregion

        #region Unity Methods

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }

        #endregion

    }
}