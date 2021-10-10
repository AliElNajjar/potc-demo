using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class DamageDisplay : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI displayText;
        public float yMovement = -10;
        public float alphaChange = 0.1f;

        #endregion

        #region Unity Methods

        private void Update()
        {
            Color c = displayText.color;
            c.a = Mathf.Clamp(c.a - alphaChange * Time.deltaTime, 0, 1);

            displayText.color = c;
            GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, yMovement * Time.deltaTime);

            if (c.a == 0)
            {
                Destroy(gameObject);
            }
        }

        #endregion

    }
}