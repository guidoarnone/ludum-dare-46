using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Army : MonoBehaviour {

    public bool empty { get { return battleValue == 0; } }

    public float front {
        get {
            for (int i = squads.Length - 1; i >= 0; i--) {
                Squad S = squads[i];
                if (!S.empty) { return S.front; }
            }
            return squads[0].back;
        }
    }

    public int battleValue { get; protected set; }

    [SerializeField]
    protected Text battleValueText;

    [SerializeField]
    protected Squad[] squads;

    public void awake() {
        battleValue = 0;
        for (int i = 0; i < squads.Length; i++) { 
            Squad S = squads[i];
            S.awake();
            S.change += updateBattleValue;
        }
    }

    public void reset() {
        for (int i = 0; i < squads.Length; i++) {
            Squad S = squads[i];
            S.reset();
        }
    }

    public void add(int n) {
        squads[0].add(n);
    }

    public void remove(int n) {
        squads[0].remove(n);
    }

    void updateBattleValue(int delta) {
        battleValue += delta;
        battleValueText.text = battleValue.ToString();
    }

    public static implicit operator int (Army A) {
        return A.battleValue;
    }

    public static float distance (Army A, Army B) {
        return Math.abs(A.front - B.front);
    }
}
