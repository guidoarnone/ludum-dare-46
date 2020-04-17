using UnityEngine;

public static class Math {

    public const float pi = 3.14159274f;
    public const float deg2rad = 0.0174532924f;
    public const float rad2deg = 57.29578f;

    /// <summary>
    /// Returns the absolute value of a number.
    /// </summary>
    public static float abs (float value) {
        if (value < 0) {
            return -value;
        }
        return value;
    }

    /// <summary>
    /// Returns the absolute value of a number.
    /// </summary>
    public static int abs (int value) {
        if (value < 0) {
            return -value;
        }
        return value;
    }

    /// <summary>
    /// Returns the minimum value between two numbers.
    /// </summary>
    public static float min (float value, float min) {
        return value < min ? value : min;
    }

    /// <summary>
    /// Returns the minimum value between two numbers.
    /// </summary>
    public static int min (int value, int min) {
        return value < min ? value : min;
    }

    /// <summary>
    /// Returns the maximum value between two numbers.
    /// </summary>
    public static float max (float a, float b) {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the maximum value between two numbers.
    /// </summary>
    public static int max (int a, int b) {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified number.
    /// </summary>
    public static int floor (float value) {
        return (int)(value - (value % 1));
    }

    /// <summary>
    /// Returns the smallest integral value greater than or equal to the specified number.
    /// </summary>
    public static int ceil (float value) {
        if (value % 1 == 0f) {
            return (int) value;
        }
        return value < 0 ?
        (int)(value) :
        (int)(value + 1f);
    }

    /// <summary>
    /// Returns the fractional part of a floating point number. 
    /// </summary>
    public static float frac (float value) {
        return value % 1;
    }

    /// <summary>
    /// Returns <paramref name="value"/> to the <paramref name="power"/> power. 
    /// </summary>
    /// <param name="value">The value being exponenciated.</param>
    /// <param name="power">The exponent.</param>
    public static int pow(int value, int power) {
        int number = value;
        for (int i = 0; i < power; i++) {
            number *= value;
        }
        return number;
    }

    /// <summary>
    /// Returns <paramref name="value"/> to the <paramref name="power"/> power. 
    /// </summary>
    /// <param name="value">The value being exponenciated.</param>
    /// <param name="power">The exponent.</param>
    public static float pow(float value, int power) {
        float number = value;
        for (int i = 0; i < power; i++) {
            number *= value;
        }
        return number;
    }

    /// <summary>
    /// Rounds a value to the nearest integer.
    /// </summary>
    public static int round (float value) {
        return frac(value) >= 0.5f ? ceil(value) : floor(value) ;
    }

    /// <summary>
    /// Returns an integer that indicates the sign of a number.
    /// </summary>
    public static float sign (float value) {
        return value < 0 ? -1 : 1;
    }

    /// <summary>
    /// Returns an integer that indicates the sign of a number.
    /// </summary>
    public static int sign (int value) {
        return value < 0 ? -1 : 1;
    }

    /// <summary>
    /// Returns a clipped <paramref name="value"/>, being greater or equal to <paramref name="min"/> and less or equal to <paramref name="max"/>.
    /// </summary>
    /// <param name="value">The value being clipped.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    public static float clamp (float value, float min, float max) {
        value = value > max ? max : value;
        return value < min ? min : value;
    }

    /// <summary>
    /// Returns a clipped <paramref name="value"/>, being greater or equal to <paramref name="min"/> and less or equal to <paramref name="max"/>.
    /// </summary>
    /// <param name="value">The value being clipped.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    public static int clamp (int value, int min, int max) {
        value = value > max ? max : value;
        return value < min ? min : value;
    }

     /// <summary>
    /// Returns a clipped <paramref name="value"/>, being greater or equal to <paramref name="min"/> and less or equal to <paramref name="max"/>.
    /// </summary>
    /// <param name="value">The value being clipped.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    public static Int2 clamp (Int2 value, Int2 min, Int2 max) {
        int x = clamp(value.x, min.x, max.x);
        int y = clamp(value.y, min.y, max.y);
        return new Int2(x, y);
    }

    /// <summary>
    /// Returns a clipped value, being greater or equal to <c>0</c>  and less or equal to <c>1</c>.
    /// </summary>
    /// <param name="value">The value being clipped.</param>
    public static float clamp01 (float value) {
        value = value > 1 ? 1 : value;
        return value < 0 ? 0 : value;
    }

    /// <summary>
    /// Returns a clipped value, being greater or equal to <c>0</c>  and less or equal to <c>1</c>.
    /// </summary>
    /// <param name="value">The value being clipped.</param>
    public static int clamp01 (int value) {
        value = value > 1 ? 1 : value;
        return value < 0 ? 0 : value;
    }

    /// <summary>
    /// Linearly interpolates between <paramref name="a"/>and <paramref name="b"/>by <paramref name="t"/>.
    /// </summary>
    /// <param name="a">Start value.</param>
    /// <param name="b">End value.</param>
    /// <param name="t">Interpolation value.</param>
    public static float lerp (float a, float b, float t) {
        return a + ( b - a ) * clamp01(t);
    }

    /// <summary>
    /// Interpolates smoothly between <paramref name="a"/>and <paramref name="b"/>by <paramref name="t"/>.
    /// </summary>
    /// <param name="a">Start value.</param>
    /// <param name="b">End value.</param>
    /// <param name="t">Interpolation value.</param>
    public static float smoothstep (float a, float b, float t) {
        t = t * t * ( 3f - 2f * t );
        return lerp(a, b, t);
    }

    /// <summary>
    /// Interpolates extra smoothly between <paramref name="a"/>and <paramref name="b"/>by <paramref name="t"/>.
    /// </summary>
    /// <param name="a">Start value.</param>
    /// <param name="b">End value.</param>
    /// <param name="t">Interpolation value.</param>
    public static float smootheststep (float a, float b, float t) {
        t = t * t * t * ( t * ( 6f * t - 15f ) + 10f );
        return lerp(a, b, t);
    }

    public static Vector3 set_vector_length (Vector3 vector, float size) {
        return Vector3.Normalize(vector) * size;
    }

}