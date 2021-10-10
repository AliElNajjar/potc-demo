using UnityEngine;

namespace NullSave.TOCK
{
    public class EnemySpawner : MonoBehaviour
    {

        #region Variables

        public float spawnTime = 0;
        public GameObject objectToSpawn;
        public bool destroyAfterSpawn = true;

        private float timeRemaining;

        #endregion

        #region Unity Methods

        private void Start()
        {
            timeRemaining = spawnTime;
        }

        private void Update()
        {
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0)
            {
                GameObject spawn = Instantiate(objectToSpawn);
                spawn.transform.position = transform.position;
                spawn.transform.rotation = transform.rotation;

                if (destroyAfterSpawn)
                {
                    Destroy(gameObject);
                }
                else
                {
                    timeRemaining = spawnTime;
                }
            }
        }

        #endregion

    }
}