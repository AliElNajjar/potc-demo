using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK
{
    [HierarchyIcon("tock-wait", false)]
    public class TriggerAfterTime : MonoBehaviour
    {

        #region Variables

        public float timeDelay = 1;
        public bool loop = false;

        public UnityEvent onTimeElapsed;

        private float elapsed;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            elapsed = 0;
            if (timeDelay == 0)
            {
                onTimeElapsed?.Invoke();
                enabled = false;
            }
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= timeDelay)
            {
                onTimeElapsed?.Invoke();
                if (loop)
                {
                    elapsed = 0;
                }
                else
                {
                    enabled = false;
                }
            }
        }

        #endregion

    }
}