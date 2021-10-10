using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Stats
{
    public class EffectDisplay : MonoBehaviour
    {

        #region Variables

        public Image spriteImage;
        public Text nameText;
        public Text descriptionText;
        public Text remainingTimeText;

        public StatEffect targetEffect;

        #endregion

        #region Unity Methods

        public void Update()
        {
            if (targetEffect == null) return;

            spriteImage.sprite = targetEffect.sprite;
            nameText.text = targetEffect.displayName;
            descriptionText.text = targetEffect.description;

            if (targetEffect.hasLifeSpan)
            {
                remainingTimeText.text = Mathf.CeilToInt(targetEffect.RemainingTime).ToString();
            }
            else
            {
                remainingTimeText.text = string.Empty;
            }

        }

        #endregion

    }
}