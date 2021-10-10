using System;
using UnityEngine;

namespace NullSave.TOCK
{
    [Serializable]
    public class DamageResistance
    {

        #region Variables

        public string damageType;
        [Range(0,1)] public float damageResistance;

        #endregion

    }
}
