using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Stats
{
    public class EffectMonitor : MonoBehaviour
    {

        #region Variables

        public Image effectImage;
        public TextMeshProUGUI effectName, effectTime;

        private StatEffect effect;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (effect != null && effect.hasLifeSpan)
            {
                effectTime.text = Mathf.CeilToInt(effect.RemainingTime) + "sec";
                if (effect.RemainingTime <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }

        #endregion

        #region Public Methods

        public void SetEffect(StatEffect statEffect)
        {
            effect = statEffect;
            effectImage.sprite = effect.sprite;
            effectName.text = effect.displayName;
            if (effect.hasLifeSpan)
            {
                effectTime.text = Mathf.CeilToInt(effect.RemainingTime) + "sec";
            }
            else
            {
                effectTime.text = string.Empty;
            }
        }

        #endregion


    }
}