﻿using UnityEngine;
using UnityEditor;
using Geometry;

public class Squad : MonoBehaviour {

    public event ValueChange change = delegate { };

    //TODO on 3D
    public float front { get { Vector3Int? C = current; return C == null ? back : position(current.Value).z; } }

    public float back { get { return position(0, 0, 0).z; } }

    public int battleValue { get; protected set; }

    public bool empty { get { return number == 0; } }

    public bool full { get { return number == capacity; } }

    public int number { get; protected set; }

    public int capacity { get { return squadSize.x * squadSize.y * squadSize.z; } }

    public Vector3Int? coordinate(int n) {
        if (n < 0 || n >= capacity) { return null; }
        int x, y, z;
        x = n % squadSize.x;
        y = n / (squadSize.x * squadSize.z);
        z = (n / squadSize.x) % squadSize.z;
        return new Vector3Int(x, y, z);
    }

    public Vector3Int? current { get { return coordinate(number - 1); } }

    public Vector3Int? next { get { return coordinate(number); } }

    public bool this[int x, int y, int z] {
        get { return units[x, y, z].isActiveAndEnabled; }
        set {
            if (units[x, y, z].isActiveAndEnabled == value) { return; }
            units[x, y, z].gameObject.SetActive(value);
            int delta;
            if (units[x, y, z].isActiveAndEnabled) { delta = units[x, y, z].battleValue; number++; }
            else { delta = -units[x, y, z].battleValue; number--; }
            battleValue += delta;
            change(delta);
        }
    }

    [SerializeField]
    protected Unit unit;

    [SerializeField]
    protected Squad previousSquad;

    [SerializeField]
    protected Squad nextSquad;

    [SerializeField]
    protected Vector3Int squadSize;

    [SerializeField]
    protected float unitRadius;

    [SerializeField]
    protected float unitBuffer;

    [SerializeField]
    protected Unit[,,] units;

    public void awake() {
        units = new Unit[squadSize.x, squadSize.y, squadSize.z];
        for (int x = 0; x < squadSize.x; x++) {
            for (int y = 0; y < squadSize.y; y++) {
                for (int z = 0; z < squadSize.z; z++) {
                    units[x, y, z] = Instantiate(unit, position(x, y, z), transform.rotation, transform);
                    units[x, y, z].gameObject.SetActive(false);
                }
            }
        }

        number = 0;
        battleValue = 0;
    }

    public void reset() {
        clear();
        number = 0;
        battleValue = 0;
    }

    public Vector3 position(Vector3Int V) {
        return position(V.x, V.y, V.z);
    }

    public Vector3 position(int x, int y, int z) {
        Int2 squadSizeGround = new Int2(squadSize.x, squadSize.z);
        Vector2 size = (squadSizeGround * (unitRadius + unitBuffer / 2f));
        Vector3 offset = new Vector3(-size.x + unitBuffer / 2f + unitRadius + x * (unitBuffer + unitRadius * 2), unitRadius + y * (unitBuffer + unitRadius * 2), -size.y + unitBuffer / 2f + unitRadius + z * (unitBuffer + unitRadius * 2));
        return transform.TransformPoint(offset);
    }

    protected void place(int x, int y, int z) {
        units[x, y, z].transform.position = position(x, y, z);
    }

    public void add(int n) {
        if (nextSquad) {nextSquad.add(n/capacity); }
        n %= capacity;
        for (int i = 0; i < n; i++) {
            Vector3Int? index = next;
            if (index!=null) {
                this[index.Value.x, index.Value.y, index.Value.z] = true;
            }
            if (number >= capacity && nextSquad && !nextSquad.full) { clear(); nextSquad.add(1); }
        }
    }

    public void remove(int n) {
        if (nextSquad) { nextSquad.remove(n/capacity); }
        n %= capacity;
        if (n>number && nextSquad && !nextSquad.empty) { nextSquad.remove(1); add(capacity - n); return; }
        for (int i = 0; i < n; i++) {
            Vector3Int? index = current;
            if (index != null) {
                this[index.Value.x, index.Value.y, index.Value.z] = false;
            }
            else { break;  }
        }
    }

    protected void clear() {
        int n = number;
        for (int i = 0; i < n; i++) { 
            Vector3Int? index = current;
            if (index == null) { return; }
            this[index.Value.x, index.Value.y, index.Value.z] = false;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Int2 squadSizeGround = new Int2(squadSize.x, squadSize.z);
        Vector2 size = (squadSizeGround * (unitRadius + unitBuffer/2f));
        Matrix4x4 m = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Draw.Gizmo.grid(new Rectangle(Vector3.zero, Vector3.up, Vector3.forward, size), squadSizeGround);
        Gizmos.matrix = m;
        int x = 0, y = 0, z = 0;
        for (x = 0; x < squadSize.x; x++) {
            for (z = 0; z < squadSize.z; z++) {
                Vector3 offset = new Vector3(-size.x+unitBuffer/2f+unitRadius+x*(unitBuffer+unitRadius*2), unitRadius, -size.y+unitBuffer/2f+unitRadius+z*(unitBuffer+unitRadius*2));
                Gizmos.color = Color.white;
                Draw.Gizmo.sphere(new Sphere(position(x, y, z), unitRadius), 16);
                Gizmos.color = Color.gray;
                for (y = 1; y < squadSize.y; y++) {
                    offset.y += unitRadius*2 + unitBuffer;
                    Draw.Gizmo.marker(new Sphere(transform.position + offset, unitRadius/2f));
                }
                y = 0;
            }
        }
    }
}
