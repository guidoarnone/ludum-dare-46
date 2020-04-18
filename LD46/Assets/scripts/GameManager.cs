using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public WeaponManager weaponManager;
    public static IncomeManager incomeManager;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Draw.Gizmo.sphere(new Geometry.Sphere(transform.position, 2.5f), 16);
        Draw.Gizmo.marker(new Geometry.Sphere(transform.position, 10f));
    }

    void Start() {
      incomeManager = new IncomeManager();
      incomeManager.setSeedIncrement(5);
      incomeManager.setSeedInterval(1);
    }

    void Update() {
      incomeManager.update();
      Debug.Log(incomeManager.seeds);
    }

    void singleton() { instance = instance ?? this; }
}
