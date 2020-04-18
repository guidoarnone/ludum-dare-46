using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static IncomeManager incomeManager;

    void Start() {
      incomeManager = new IncomeManager();
      //debugging
      incomeManager.setSeedIncrement(5);
      incomeManager.setSeedInterval(1);
    }

    void Update() {
      incomeManager.update();
      Debug.Log(incomeManager.seeds);
    }
}
