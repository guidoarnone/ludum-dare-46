using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Path : MonoBehaviour {
    public abstract Vector3 getPosition(float t);
}