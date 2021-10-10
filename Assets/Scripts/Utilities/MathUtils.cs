using UnityEngine;

/// <summary>
/// Utility class to use math functions.
/// </summary>
public static class MathUtils
{
    /// <summary>
    ///  Returns true if float a is approximately equal to float b, with an epsilon parameter for floating point issues.
    /// </summary>
    /// <param name="a">First value to compare.</param>
    /// <param name="b">Second value to compare.</param>
    /// <param name="epsilon">Aceptable interval between them.</param>
    /// <example>
    /// Here's a comparison between two vectors to see if they're the same.
    ///     <code>
    ///     float a = 0.55f;
    ///     float b = 0.60f;
    /// 
    ///     bool areEqual = MathHelper.Approximately(a, b, 0.05f);
    /// 
    ///     print("Floats are equal " + areEqual);
    ///     </code>
    /// </example>
    public static bool Approximately(float a, float b, float epsilon)
    {
        return (Mathf.Abs(a - b) < epsilon);
    }

    /// <summary>
    /// Returns true if vector 1 is approximately equal to vector 2, with an epsilon parameter for floating point issues.
    /// </summary>
    /// <param name="v1">First vector to compare.</param>
    /// <param name="v2">Second vector to compare.</param>
    /// <param name="epsilon">Aceptable interval between them.</param>
    /// <example>
    /// Here's a comparison between two vectors to see if they're the same.
    ///     <code>
    ///     Vector3 vectorOne = new Vector3(0, 0.9f, 0);
    ///     Vector3 vectorTwo = new Vector3(0, 0.5f, 0);
    /// 
    ///     bool areEqual = MathHelper.Approximately(vectorOne, vectorTwo, 0.5f);
    /// 
    ///     print("Vectors are equal " + areEqual);
    ///     </code>
    /// </example>
    public static bool Approximately(Vector3 v1, Vector3 v2, float epsilon)
    {
        return Approximately(v1.x, v2.x, epsilon)
            && Approximately(v1.y, v2.y, epsilon)
            && Approximately(v1.z, v2.z, epsilon);
    }

    public static bool Approximately(Color c1, Color c2, float epsilon)
    {
        return Approximately(c1.r, c2.r, epsilon)
            && Approximately(c1.g, c2.g, epsilon)
            && Approximately(c1.b, c2.b, epsilon);
    }

    /// <summary>
    /// Returns the angle between the point and the horizontal axis.
    /// </summary>
    /// <param name="x">Value in the horizontal axis.</param>
    /// <param name="y">Value in the vertical axis.</param>
    public static float Degree(float x, float y)
    {
        double value = (System.Math.Atan2(y, x) / System.Math.PI) * 180;
        if (value < 0d) value += 360d;
        return (float)value;
    }

    /// <summary>
    /// Returns the angle in radians.
    /// </summary>
    /// <param name="angle">Angle to transform.</param>
    public static float ToRadians(float angle)
    {
        return angle * Mathf.PI / 180f;
    }

    public static Vector2 SmoothStep(Vector2 from, Vector2 to, float t)
    {
        float x = Mathf.SmoothStep(from.x, to.x, t);
        float y = Mathf.SmoothStep(from.y, to.y, t);
        return new Vector2(x, y);
    }
}