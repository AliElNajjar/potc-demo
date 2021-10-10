using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK
{
    public class MessageListItem : MonoBehaviour
    {

        #region Variables

        public Image icon;
        public bool fadeIconAlpha = true;

        public TextMeshProUGUI message;
        public bool fadeMessageAlpha = true;

        public Image background;
        public bool fadeBackgroundAlpha = true;

        public float timeBeforeFade = 2;
        public float lifeTime = 2;

        private float elapsed, beforeElapsed;
        private float orgAlphaIcon, orgAlphaMessage, orgAlphaBkg;
        private bool messageSet;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (!messageSet) return;

            if (beforeElapsed <= timeBeforeFade)
            {
                beforeElapsed += Time.deltaTime;
                return;
            }

            elapsed = Mathf.Clamp(elapsed + Time.deltaTime, elapsed, lifeTime);
            float progress = elapsed / lifeTime;

            if (icon != null && fadeIconAlpha)
            {
                icon.color = FadeAlpha(icon.color, orgAlphaIcon, progress);
            }

            if (message != null && fadeMessageAlpha)
            {
                message.color = FadeAlpha(message.color, orgAlphaMessage, progress);
            }

            if (background != null && fadeBackgroundAlpha)
            {
                background.color = FadeAlpha(background.color, orgAlphaBkg, progress);
            }

            if (progress == 1)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Public Methods

        public void SetMessage(Sprite sprite, string text)
        {
            if (icon != null)
            {
                icon.sprite = sprite;
                orgAlphaIcon = icon.color.a;
                if (icon.sprite == null)
                {
                    icon.gameObject.SetActive(false);
                }
            }

            if (message != null)
            {
                message.text = text;
                orgAlphaMessage = message.color.a;
            }

            if (background != null)
            {
                orgAlphaBkg = background.color.a;
            }

            messageSet = true;
            elapsed = 0;
            beforeElapsed = 0;
        }

        #endregion

        #region Private Methods

        private Color FadeAlpha(Color color, float orgAlpha, float progress)
        {
            orgAlpha = orgAlpha - (orgAlpha * progress);
            return new Color(color.r, color.g, color.b, orgAlpha);
        }

        #endregion

    }
}