using System.Collections;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [DefaultExecutionOrder(1)]
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {

        #region Variables

        public float speed = 1f;
        public bool autoLaunch = false;
        public float maxLife = 6;

        private bool launched;
        private float elapsed;
        private DamageDealer[] dealers;
        private bool damageDealt;

        private StatsCog statsCog;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            dealers = GetComponentsInChildren<DamageDealer>();
            foreach (DamageDealer dealer in dealers)
            {
                if (statsCog == null) statsCog = dealer.StatsSource;
                dealer.onDamageDealt.AddListener(DamageDealt);
            }
        }

        private void Start()
        {
            if (autoLaunch)
            {
                StartCoroutine("ProjectileControl");
            }
        }

        #endregion

        #region Public Methods

        public void Launch()
        {
            transform.SetParent(null);
            StartCoroutine("ProjectileControl");
        }

        #endregion

        #region Private Methods

        private void DamageDealt(float amount, DamageReceiver target)
        {
            if (amount == 0 || target.StatsParent == statsCog) return;
            damageDealt = true;
        }

        private IEnumerator ProjectileControl()
        {
            if (!launched)
            {
                launched = true;

                while (!damageDealt && elapsed < maxLife)
                {
                    transform.position += transform.forward * speed * Time.deltaTime;
                    elapsed += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                Destroy(gameObject);
            }
        }

        #endregion

    }
}