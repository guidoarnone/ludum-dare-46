using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CombatEnd(Army A);

public class CombatManager : MonoBehaviour {

    const float hitInterval = 3f;

    protected bool produce { set {
            if (value) {
                GameManager.instance.incomeManager.battleIncome.income += playerArmy.add;
                GameManager.instance.incomeManager.battleIncome.income += enemyArmy.add;
            }
            else {
                GameManager.instance.incomeManager.battleIncome.income -= playerArmy.add;
                GameManager.instance.incomeManager.battleIncome.income -= enemyArmy.add;
            }
        }
    }

    [SerializeField]
    protected Path path;

    [SerializeField]
    public Army playerArmy;

    [SerializeField]
    public Army enemyArmy;

    public AnimationCurve hitCurve;

    protected float startTime = -1;

    public void awake() {
        playerArmy.awake();
        enemyArmy.awake();
        produce = true;
    }

    public void update() {
        Debug.Log(Army.distance(playerArmy, enemyArmy));
        float v = 0;
        if ((startTime+hitInterval) >= Time.time) {
            float t = (Time.time - startTime) / hitInterval;
            v = hitCurve.Evaluate(t);
        }
        Shader.SetGlobalFloat("_RushHour", v);
    }

    public void battle() {
        produce = false;
        Battle B = new Battle(playerArmy, enemyArmy, winner);
        StartCoroutine("hit", B);
    }

    IEnumerator hit(Battle B) {
        while (true) {
            startTime = Time.time;
            yield return new WaitForSeconds(hitInterval/2f);
            if (B.hit()) { yield return new WaitForSeconds(hitInterval / 2f); break; }
            yield return new WaitForSeconds(hitInterval/2f);
        }
    }

    protected void winner(Army A) {
        if (A != enemyArmy) {
            enemyArmy.reset();
            produce = true;
        }
        else { GameManager.instance.gameOver(); }
    }
}
