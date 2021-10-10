using UnityEngine;

namespace NullSave.TOCK
{
    [HierarchyIcon("tock-wait", false)]
    public class ObjectSpawner : MonoBehaviour
    {

        #region Variables

        public GameObject spawnPrefab;
        public bool parentToCurrent;

        public float timeDelay = 1;
        public bool loop = false;

        private float elapsed;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            elapsed = 0;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= timeDelay)
            {
                Instantiate(spawnPrefab, parentToCurrent ? transform : null);

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