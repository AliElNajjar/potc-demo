using UnityEngine;

namespace NullSave.TOCK
{
    public static class MathExtensions
    {

        #region Public Methods

        public static float ClampAngle(float angle, float min, float max)
        {
            do
            {
                if (angle < -360) angle += 360;
                if (angle > 360) angle -= 360;
            } while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }

        public static float Convert360to180(float angle)
        {
            if (angle < -180) return angle + 360;
            if (angle > 180) return angle - 360;
            return angle;
        }

        public static float Lerp2(this float a, float b, float t)
        {
            if (t > 1) t = 1;
            if (a < b)
            {
                return a + ((b - a) * t);
            }
            else
            {
                return b - ((a - b) * t);
            }
        }

        /// <summary>
        /// Normalized the angle. between -180 and 180 degrees
        /// </summary>
        /// <param Name="eulerAngle">Euler angle.</param>
        public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
        {
            var delta = eulerAngle;

            delta.x = Convert360to180(delta.x);
            delta.y = Convert360to180(delta.y);
            delta.z = Convert360to180(delta.z);

            return new Vector3(delta.x, delta.y, delta.z);
        }

        public static bool PointInQuad(Vector2 point, Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
        {
            if (PointInTriangle(point, topLeft, topRight, bottomLeft)) return true;
            if (PointInTriangle(point, topRight, bottomRight, bottomLeft)) return true;
            return false;
        }

        public static bool PointInTriangle(Vector2 point, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = PointSign(point, v1, v2);
            d2 = PointSign(point, v2, v3);
            d3 = PointSign(point, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        #endregion

        #region Private Methods

        private static float PointSign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        #endregion
    }
}