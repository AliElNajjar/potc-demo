using UnityEngine;

namespace NullSave.TOCK
{
    public class DieAfterTime : MonoBehaviour
    {

        #region Variables

        public float lifeInSeconds = 3f;

        private float remainingLife;

        #endregion

        #region Unity Methods

        public void Start()
        {
            remainingLife = lifeInSeconds;
        }

        public void Update()
        {
            remainingLife -= Time.deltaTime;
            if (remainingLife <= 0)
            {
                Destroy(gameObject);
            }
        }

        #endregion

    }
}