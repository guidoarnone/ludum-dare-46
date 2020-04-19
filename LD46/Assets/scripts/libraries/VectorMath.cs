using UnityEngine;

[System.Serializable]
public struct Segment2 {

    public Vector2 direction { get { return (B - A).normalized; } }

    public Vector2 A;
    public Vector2 B;
    public float length;

    public Segment2(Vector2 A, Vector2 B) {
        this.A = A;
        this.B = B;
        length = Vector2.Distance(B, A);
    }

    public void translate(Vector2 V) {
        A += V;
        B += V;
    }
}

[System.Serializable]
public struct Segment3 {

    public Vector3 direction { get { return B - A; } }

    public Vector3 A;
    public Vector3 B;
    public float length;

    public Segment3(Vector3 A, Vector3 B) {
        this.A = A;
        this.B = B;
        length = Vector3.Distance(B, A);
    }

    public void translate(Vector3 V) {
        A += V;
        B += V;
    }
}

public static class VectorMath {

    public static Vector2 intersect(Segment2 L1, Segment2 L2) {
        float A1 = L1.B.y - L1.A.y;
        float B1 = L1.A.x - L1.B.x;
        float C1 = A1*L1.A.x+B1*L1.A.y;

        float A2 = L2.B.y - L2.A.y;
        float B2 = L2.A.x - L2.B.x;
        float C2 = A1 * L2.A.x + B1 * L2.A.y;

        float determinant = A1*B2-A2*B1;
        if (determinant == 0) { return Vector2.zero; }
        float x = (B2*C1-B1*C2)/determinant;
        float y = (A1*C2-A2*C1)/determinant;
        return new Vector2(x, y);
    }

    public static Vector3 clamp(Vector3 V, Vector3 min, Vector3 max) {
        return new Vector3(
            Math.clamp(V.x, min.x, max.x),
            Math.clamp(V.y, min.y, max.y),
            Math.clamp(V.z, min.z, max.z));
    }
}