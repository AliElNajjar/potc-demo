using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class ProjectileLauncher : MonoBehaviour
    {

        #region Variables

        public Projectile projectile;
        public Transform projectileSpawn;
        public bool autoLaunch = false;
        public float launchInterval = 5f;
        public float spawnDelay = 1f;

        private float launchElapsed, spawnElapsed;
        private Projectile curProjectile;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (autoLaunch)
            {
                if (curProjectile == null)
                {
                    if (spawnElapsed >= spawnDelay)
                    {
                        curProjectile = Instantiate(projectile, projectileSpawn);
                        spawnElapsed = 0;
                    }
                }

                launchElapsed += Time.deltaTime;
                if (launchElapsed >= launchInterval)
                {
                    Launch();
                }
            }
        }

        #endregion

        #region Private Methods

        public void Launch()
        {
            if (curProjectile == null)
            {
                curProjectile = Instantiate(projectile, projectileSpawn);
            }

            if(projectileSpawn != null)
            {
                curProjectile.transform.localRotation = Quaternion.Euler(Vector3.zero);
                curProjectile.transform.localPosition = Vector3.zero;
            }
            else
            {
                curProjectile.transform.localRotation = transform.rotation;
                curProjectile.transform.localPosition = transform.position;
            }

            curProjectile.Launch();
            curProjectile = null;
            launchElapsed = 0;
        }

        #endregion

    }
}