using TMPro;
using UnityEngine;

namespace NullSave.TOCK
{
    public class InteractPrompt : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI target;
        public bool autoSize = true;
        public Padding padding;

        #endregion

        #region Public Methods

        public void SetText(string text)
        {
            if (autoSize)
            {
                Vector2 bodySize = target.GetTMPRequiredSize(text, new Vector2(1200, 1200));
                bodySize.x += padding.Horizontal;
                bodySize.y += padding.Vertical;
                GetComponent<RectTransform>().sizeDelta = bodySize;
            }
            else
            {
                target.text = text;
            }
        }

        #endregion

    }
}