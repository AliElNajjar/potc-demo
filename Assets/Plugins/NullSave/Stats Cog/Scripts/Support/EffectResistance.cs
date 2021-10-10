using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [System.Serializable]
    public class EffectResistance
    {

        #region Variables

        public StatEffect effect;
        [Range(0, 1)] public float resistChance;

        #endregion

    }
}