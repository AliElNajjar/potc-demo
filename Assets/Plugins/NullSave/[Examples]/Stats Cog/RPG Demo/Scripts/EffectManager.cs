using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Stats
{
    public class EffectManager : MonoBehaviour
    {

        #region Variables

        public StatsCog statsCog;
        public Dropdown effectsDD;

        #endregion

        #region Public Methods

        public void AddEffect()
        {
            statsCog.AddEffect(effectsDD.captionText.text);
        }

        public void ClearEffects()
        {
            statsCog.ClearEffects();
        }

        public void RemoveEffect()
        {
            statsCog.RemoveEffect(effectsDD.captionText.text);
        }

        #endregion

    }
}