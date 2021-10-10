using TMPro;
using UnityEngine;

namespace NullSave.TOCK
{
	public static class SharedExtensions
	{

        #region Public Methods

		public static bool IsNumeric (this string value)
		{
			float val;
            return float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out val);
		}

        public static void RotateAround(this Transform transform, Transform reference)
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, reference.eulerAngles.y, transform.eulerAngles.z));
        }

        public static bool ToBool(this string value)
        {
            if (value.ToLower() == "true") return true;

            int iVal;
            if (int.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out iVal))
            {
                return iVal != 0;
            }

            return false;
        }

        public static Vector2 ToVector2(this string value)
        {
            string[] parts = value.Split(',');
            return new Vector2(float.Parse(parts[0].Trim()), float.Parse(parts[1].Trim()));
        }

        public static Vector3 ToVector3(this string value)
        {
            string[] parts = value.Split(',');
            return new Vector3(float.Parse(parts[0].Trim()), float.Parse(parts[1].Trim()), float.Parse(parts[2].Trim()));
        }

        #endregion

    }

    public static class TMProExtensions
    {

        public static Vector2 GetTMPRequiredSize(this TextMeshProUGUI target, string text, Vector2 maxSize)
        {
            target.text = text;

            RectTransform rt = target.GetComponent<RectTransform>();
            Vector2 orgMax = rt.anchorMax;
            Vector2 orgMin = rt.anchorMin;
            Vector2 orgSize = rt.sizeDelta;

            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = maxSize;

            target.ForceMeshUpdate();

            rt.anchorMax = orgMax;
            rt.anchorMin = orgMin;
            rt.sizeDelta = orgSize;

            return target.textBounds.size;
        }

    }

}