using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [HierarchyIcon("damage", false)]
    public class DamageReceiver : MonoBehaviour
    {

        #region Variables

        public DamageRecieved onTakeDamage;

        #endregion

        #region Properties

        public StatsCog StatsParent { get; set; }

        #endregion

        #region Public Methods

        public virtual float TakeDamage(DamageDealer damageDealer, GameObject damageSourceObject) { onTakeDamage?.Invoke(damageDealer, damageSourceObject); return 0; }

        #endregion

    }
}