using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Action();

public class GameManager : MonoBehaviour {

    public event Action update = delegate { };

    public static GameManager instance;

    public WeaponManager weaponManager;
    public static IncomeManager incomeManager;

    [SerializeField]
    protected Army playerArmy;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Draw.Gizmo.sphere(new Geometry.Sphere(transform.position, 2.5f), 16);
        Draw.Gizmo.marker(new Geometry.Sphere(transform.position, 10f));
    }

    void Awake() {
        incomeManager = new IncomeManager();
        playerArmy.awake();

        update += incomeManager.update;
        incomeManager.battleIncome.income += playerArmy.add;

        //DEBUG
        //incomeManager.battleIncome.income += log;
    }

    void Update() {
        update();
    }

    public void log (int n) {
        Debug.Log(n);
    }

    void singleton() { instance = instance ?? this; }
}
