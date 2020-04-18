using UnityEngine;
using UnityEditor;
using Geometry;

public class Squad : MonoBehaviour {

    public Unit this[int x, int y, int z]{ get { return units[x, y, z]; } set { } }

    [SerializeField]
    protected Vector3Int squadSize;

    [SerializeField]
    protected float unitRadius;

    [SerializeField]
    protected float unitBuffer;

    [SerializeField]
    protected Unit[,,] units;

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Int2 squadSizeGround = new Int2(squadSize.x, squadSize.z);
        Vector2 size = (squadSizeGround * (unitRadius + unitBuffer/2f));
        Draw.Gizmo.grid(new Rectangle(transform.position, transform.up, transform.forward, size), squadSizeGround);

        for (int x = 0; x < squadSize.x; x++) {
            for (int y = 0; y < squadSize.y; y++) {
                Vector3 offset = new Vector3(-size.x+unitBuffer/2f+unitRadius+x*(unitBuffer+unitRadius*2), unitRadius, -size.y+unitBuffer/2f+unitRadius+y*(unitBuffer+unitRadius*2));
                Gizmos.color = Color.white;
                Draw.Gizmo.sphere(new Sphere(transform.position + offset, unitRadius), 16);
                Gizmos.color = Color.gray;
                for (int z = 1; z < squadSize.y; z++) {
                    offset.y += unitRadius*2 + unitBuffer;
                    Draw.Gizmo.marker(new Sphere(transform.position + offset, unitRadius/2f));
                }
            }
        }
    }
}
