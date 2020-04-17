using System;
using UnityEngine;

[Serializable]
public class Int2 {

    public static Int2 zero { get { return new Int2(0, 0); } }
    public static Int2 one { get { return new Int2(1, 1); } }
    public static Int2 up { get { return new Int2(0, 1); } }
    public static Int2 right { get { return new Int2(1, 0); } }
    public static Int2 down { get { return new Int2(0, -1); } }
    public static Int2 left { get { return new Int2(-1, 0); } }

    public int x;
    public int y;

    public int a {
        get { return x; }
        set { x = value; }
    }

    public int b {
        get { return y; }
        set { y = value; }
    }

    public int r {
        get { return x; }
        set { x = value; }
    }

    public int g {
        get { return y; }
        set { y = value; }
    }

    public Int2 (int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Int2 (Int2 int2) {
        this.x = int2.x;
        this.y = int2.y;
    }

    public int sum { get { return x + y; } }

    public int product { get { return x * y; } }

    public static Int2 operator + (Int2 a, int b) {
        return new Int2(a.x + b, a.y + b);
    }

    public static Int2 operator + (Int2 a, Int2 b) {
        return new Int2(a.x + b.x, a.y + b.y);
    }

    public static Int2 operator - (Int2 a) {
        return new Int2(-a.x, -a.y);
    }

    public static Int2 operator - (Int2 a, int b) {
        return new Int2(a.x - b, a.y - b);
    }

    public static Int2 operator - (Int2 a, Int2 b) {
        return new Int2(a.x - b.x, a.y - b.y);
    }

    public static Int2 operator * (Int2 a, int b) {
        return new Int2(a.x * b, a.y * b);
    }

    public static Int2 operator * (Int2 a, Int2 b) {
        return new Int2(a.x * b.x, a.y * b.y);
    }

    public static Int2 operator / (Int2 a, int b) {
        return new Int2(a.x / b, a.y / b);
    }

    public static Int2 operator / (Int2 a, Int2 b) {
        return new Int2(a.x / b.x, a.y / b.y);
    }

    public static Int2 operator % (Int2 a, int b) {
        return new Int2(a.x % b, a.y % b);
    }

    public static Int2 operator % (Int2 a, Int2 b) {
        return new Int2(a.x % b.x, a.y % b.y);
    }

    public override int GetHashCode() {
        unchecked {

            const int hashBase = (int) 2166136261;
            const int HashMultiplier = 16777619;

            int hash = (hashBase * HashMultiplier) ^ a;
            hash = (hash * HashMultiplier) ^ b;

            return hash;
        }
    }

    public override bool Equals(object I2) {
        if(I2 == null) {
            return false;
        }

        Int2 value = I2 as Int2;

        return  (value != null)
                && value.a == a
                && value.b == b;
    }

    public static bool operator ==(Int2 a, Int2 b) {
        if (ReferenceEquals(a, null)) {
            if (ReferenceEquals(b, null)) { return true; }
            return false;
        }
        return a.Equals(b);
    }

    public static bool operator !=(Int2 a, Int2 b) {
        return !(a == b);
    }

    public static implicit operator Vector2 (Int2 a) {
        return new Vector2(a.x, a.y);
    }

    public static Vector2 operator * (Int2 a, float b) {
        return new Vector2(a.x * b, a.y * b);
    }

    public static Int2 distance (Int2 a, Int2 b) {
        return new Int2( Math.abs(a.x - b.x), Math.abs(a.y - b.y) );
    }
}
