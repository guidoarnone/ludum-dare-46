using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {

    const float hitInterval = 3f;

    protected bool produce { set {
            if (value) {
                GameManager.instance.incomeManager.playerTroops.income += playerArmy.add;
            }
            else {
                GameManager.instance.incomeManager.playerTroops.income -= playerArmy.add;
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

    protected float startTime = -hitInterval*2;

    bool inCombat = false;

    public void awake() {
        playerArmy.awake(0);
        enemyArmy.awake(1024);
        inCombat = false;
        produce = true;
    }

    public void reset() {
        StopAllCoroutines();
        playerArmy.reset(0);
        enemyArmy.reset(1024);
        inCombat = false;
        produce = true;
    }

    public void update() {
        float v = 0;
        if ((startTime+hitInterval) >= Time.time) {
            float t = (Time.time - startTime) / hitInterval;
            v = hitCurve.Evaluate(t);
        }
        Shader.SetGlobalFloat("_RushHour", v);
        enemyArmy.update();
        if (Army.distance(playerArmy, enemyArmy) <= 40 && !inCombat) { battle(); }
    }

    public void battle() {
        produce = false;
        Battle B = new Battle(playerArmy, enemyArmy);
        inCombat = true;
        enemyArmy.inCombat = true;
        StartCoroutine("hit", B);
    }

    IEnumerator hit(Battle B) {
        bool ongoing = true;
        int winArmy = 0;
        while (ongoing) {
            startTime = Time.time;
            playerArmy.changeEmotion(Emotion.Attack);
            yield return new WaitForSeconds(hitInterval/2f);
            winArmy = B.hit();
            playerArmy.changeEmotion(Emotion.Hit);
            if (winArmy != 0) {
                ongoing = false;
                if (winArmy != -1) { enemyArmy.reset(Math.floor(Time.time * Time.time)); }
            }
            yield return new WaitForSeconds(hitInterval / 2f);
        }
        if (winArmy == -1) { winner(enemyArmy); }
        else if (winArmy == 1) { winner(null); }
        else { winner(playerArmy); }
        yield return new WaitForSeconds(hitInterval / 2f);
        playerArmy.changeEmotion(Emotion.Neutral);
    }

    protected void winner(Army A) {
        if (A != enemyArmy) {
            playerArmy.changeEmotion(Emotion.Joy);
            inCombat = false;
            enemyArmy.inCombat = false;
            produce = true;
        }
        else { GameManager.instance.gameOver(); }
    }
}
