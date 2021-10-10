using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class EffectArea : MonoBehaviour
    {

        #region Variables

        public StatEffect areaEffect;
        public bool removeOnExit = true;
        public bool reAddOnStay = false;
        public List<StatEffect> cancelEffects;

        #endregion

    }
}