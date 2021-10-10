using NullSave.TOCK.Stats;
using UnityEngine;

namespace NullSave.TOCK
{
    [RequireComponent(typeof(Collider))]
    public class Fireball : MonoBehaviour
    {

        #region Variables

        public float speed = 0.5f;
        public float life = 1.5f;
        public AudioClip impactSound;

        #endregion

        #region Properties

        public bool IsReleased { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            GetComponent<DamageDealer>().onDamageDealt.AddListener(DamageDealt);
        }

        private void Update()
        {
            if (!IsReleased) return;

            GetComponent<Collider>().enabled = true;
            transform.position = transform.position + (transform.forward * speed);

            life -= Time.deltaTime;
            if (life <= 0)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Private Methods

        private void DamageDealt(float damage, DamageReceiver receiver)
        {
            GameObject audio = new GameObject("explosion");
            AudioSource audioSource = audio.AddComponent(typeof(AudioSource)) as AudioSource;
            audioSource.spatialBlend = 1;
            audioSource.clip = impactSound;
            audioSource.playOnAwake = true;
            audio.transform.position = transform.position;
            audioSource.Play();
            DieAfterTime die = audio.AddComponent(typeof(DieAfterTime)) as DieAfterTime;
            die.lifeInSeconds = 1f;

            Destroy(gameObject);
        }

        #endregion

    }
}