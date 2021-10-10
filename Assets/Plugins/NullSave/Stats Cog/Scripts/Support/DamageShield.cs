using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(Collider))]
    public class DamageShield : MonoBehaviour
    {

        #region Variables

        public string shieldId = "Shield";

        #endregion

        #region Properties

        public Collider ShieldCollider { get; private set; }

        public virtual StatsCog StatsSource { get; set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            ShieldCollider = GetComponentInChildren<Collider>();
        }

        #endregion

        #region Public Methods

        public void SetCollider(bool state)
        {
            ShieldCollider.enabled = state;
        }

        #endregion

    }
}