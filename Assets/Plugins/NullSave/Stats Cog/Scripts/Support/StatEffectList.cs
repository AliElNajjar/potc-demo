using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CreateAssetMenu(menuName = "TOCK/Stats Cog/Stat Effect List", order = 1)]
    public class StatEffectList : ScriptableObject
    {

        #region Variables

        public List<StatEffect> availableEffects;

        #endregion

    }
}