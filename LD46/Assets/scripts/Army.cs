using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Army : MonoBehaviour {

    public bool inCombat = false;

    public bool empty { get { return battleValue == 0; } }

    public float front {
        get {
            for (int i = squads.Length - 1; i >= 0; i--) {
                Squad S = squads[i];
                if (!S.empty) { return S.front; }
            }
            return back;
        }
    }

    public float back {
        get { return squads[0].back; }
    }

    public int battleValue { get; protected set; }

    [SerializeField]
    protected Path path;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected Squad[] squads;

    protected float t;

    public void awake(int n) {
        battleValue = 0;
        for (int i = 0; i < squads.Length; i++) { 
            Squad S = squads[i];
            S.awake();
            S.change += updateBattleValue;
        }
        t = 0;
        foreach (Squad S in squads) { t += S.empty ? 0.05f : 0f; }
        Debug.Log(t);
        inCombat = false;
        update();
        add(n);
    }

    public void reset(int n) {
        for (int i = 0; i < squads.Length; i++) {
            Squad S = squads[i];
            S.reset();
        }
        t = 0;
        foreach (Squad S in squads) { t += S.empty ? 0.05f : 0f; }
        Debug.Log(t);
        inCombat = false;
        update();
        add(n);
    }

    public void update() {
        if (!path || inCombat) { return; }
        t += speed/100 * Time.deltaTime;
        transform.position = path.getPosition(t);
    }

    public void add(int n) {
        squads[0].add(n);
    }

    public void remove(int n) {
        squads[0].remove(n);
    }

    void updateBattleValue(int delta) {
        battleValue += delta;
    }

    public static implicit operator int (Army A) {
        return A.battleValue;
    }

    public static float distance (Army A, Army B) {
        return Math.abs(A.front - B.front);
    }
}
