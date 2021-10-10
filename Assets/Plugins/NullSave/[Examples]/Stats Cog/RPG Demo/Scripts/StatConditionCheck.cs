using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    public class StatConditionCheck : MonoBehaviour
    {

        #region Variables

        public string statName;
        public string condition;
        public StatsCog statCog;

        public UnityEvent onConditionMet, onConditionNotMet;

        #endregion

        #region Unity Events

        public void Start()
        {
            StatValue stat = statCog.FindStat(statName);
            if (stat == null) return;
            stat.onValueChanged.AddListener(StatChanged);
            StatChanged(0, 0);
        }

        #endregion

        #region Private Events

        private void StatChanged(float oldValue, float newValue)
        {
            if (statCog.EvaluateCondition(condition))
            {
                if (onConditionMet != null) onConditionMet.Invoke();
            }
            else
            {
                if (onConditionNotMet != null) onConditionNotMet.Invoke();
            }
        }

        #endregion

    }
}