using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class PlayerEffectList : MonoBehaviour
    {

        #region Variables

        public Transform effectsContainer;
        public EffectMonitor effectPrefab;

        private StatsCog statsCog;

        #endregion

        #region Unity Methods

        private void Start()
        {
            statsCog = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<StatsCog>();
            statsCog.onEffectAdded.AddListener(EffectAdded);
        }

        #endregion

        #region Public Methods

        public void ResetWithLoad()
        {
            EffectMonitor[] curEffects = effectsContainer.gameObject.GetComponentsInChildren<EffectMonitor>();
            foreach (EffectMonitor monitor in curEffects)
            {
                Destroy(monitor.gameObject);
            }

            foreach (StatEffect effect in statsCog.Effects)
            {
                EffectAdded(effect);
            }
        }

        #endregion

        #region Private Methods

        private void EffectAdded(StatEffect effect)
        {
            Instantiate(effectPrefab, effectsContainer).SetEffect(effect);
        }

        #endregion

    }
}