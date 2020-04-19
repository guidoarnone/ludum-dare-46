using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Action();

public class GameManager : MonoBehaviour {

    public event Action update = delegate { };

    public static GameManager instance;

    public CombatManager combatManager;
    public WeaponManager weaponManager;
    public IncomeManager incomeManager;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Draw.Gizmo.sphere(new Geometry.Sphere(transform.position, 2.5f), 16);
        Draw.Gizmo.marker(new Geometry.Sphere(transform.position, 10f));
    }

    void Awake() {
        singleton();

        incomeManager = new IncomeManager();
        combatManager.awake();

        update += combatManager.update;
        update += incomeManager.update;
    }

    void Update() {
        update();
        if (Input.GetKeyDown(KeyCode.Space)) { combatManager.battle(); }
    }

    public void gameOver() {

    }

    void singleton() { instance = instance ?? this; }
}
