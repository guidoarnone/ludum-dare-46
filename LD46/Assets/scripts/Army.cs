using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Army : MonoBehaviour {

    public int battleValue { get; protected set; }

    [SerializeField]
    protected Text battleValueText;

    [SerializeField]
    protected Squad[] squads;

    public void awake() {
        battleValue = 0;
        foreach (Squad S in squads) {
            S.awake();
            S.change += updateBattleValue;
        }
    }

    public void add(int n) {
        squads[0].add(n);
        battleValueText.text = battleValue.ToString();
    }

    void updateBattleValue(int delta) {
        battleValue += delta;
        
    }
}
