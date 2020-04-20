using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Action();

public class GameManager : MonoBehaviour {

    public event Action update = delegate { };

    public static GameManager instance;

    public Flower flower;
    public IncomeManager incomeManager;
    public CombatManager combatManager;
    public UIManager UIManager;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Draw.Gizmo.sphere(new Geometry.Sphere(transform.position, 2.5f), 16);
        Draw.Gizmo.marker(new Geometry.Sphere(transform.position, 10f));
    }

    void Awake() {
        singleton();

        incomeManager.awake();
        combatManager.awake();
        UIManager.awake();

        update += combatManager.update;
        update += incomeManager.update;
        update += UIManager.update;
    }

    public void reset() {
        flower.reset();
        incomeManager.reset();
        combatManager.reset();
        UIManager.reset();
        Time.timeScale = 1;
    }

    void Update() {
        update();
        if (Input.GetKeyDown(KeyCode.A)) { incomeManager.seedIncome.increment += 1; }
    }

    public void gameOver() {
        flower.gameOver();
    }

    public void win() {
        combatManager.win();
        flower.win();
    }

    void singleton() { instance = instance ?? this; }
}
