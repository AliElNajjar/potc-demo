using UnityEngine;

namespace NullSave.TOCK
{
    public class TransformRotator : MonoBehaviour
    {

        #region Variables

        public Vector3 rotationSpeed;

        #endregion

        #region Unity Methods

        private void Update()
        {
            Vector3 curRot = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(curRot + (rotationSpeed * Time.deltaTime));
        }

        #endregion

    }
}