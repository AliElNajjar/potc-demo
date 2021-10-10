using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    public class TriggerByEffect : MonoBehaviour
    {

        #region Variables

        public StatsCog statsCog;
        public StatEffect effectToMonitor;

        public UnityEvent onEffectStart, onEffectOver;

        #endregion

        #region Unity Methods

        private void Start()
        {
            statsCog.onEffectAdded.AddListener(EffectAdded);
            statsCog.onEffectEnded.AddListener(EffectEnded);
            statsCog.onEffectRemoved.AddListener(EffectEnded);
        }

        #endregion

        #region Private Methods

        private void EffectAdded(StatEffect effect)
        {
            if (effect.name == effectToMonitor.name)
            {
                onEffectStart?.Invoke();
            }
        }

        private void EffectEnded(StatEffect effect)
        {
            if (effect.name == effectToMonitor.name)
            {
                onEffectOver?.Invoke();
            }
        }

        #endregion

    }
}