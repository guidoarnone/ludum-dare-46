using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Line : Path {
    [SerializeField]
    Vector3 A = new Vector3(-25, 0, 0);
    [SerializeField]
    Vector3 B = new Vector3(25, 0, 0);

    public override Vector3 getPosition(float t) {
        return transform.TransformPoint(Vector3.Lerp(A, B, t));
    }

    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(A, B);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(A, Vector3.one * 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(B, Vector3.one * 2f);
    }
}