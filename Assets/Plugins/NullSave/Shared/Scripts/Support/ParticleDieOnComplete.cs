using UnityEngine;

namespace NullSave.TOCK
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleDieOnComplete : MonoBehaviour
    {

        #region Variables

        new ParticleSystem particleSystem;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!particleSystem.IsAlive(true))
            {
                Destroy(gameObject);
                enabled = false;
            }
        }

        #endregion

    }
}