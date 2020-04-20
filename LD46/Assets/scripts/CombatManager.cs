using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {

    const float hitInterval = 3f;

    protected bool produce { set {
            if (value) {
                GameManager.instance.incomeManager.playerTroops.valueChange += playerArmy.add;
            }
            else {
                GameManager.instance.incomeManager.playerTroops.valueChange -= playerArmy.add;
            }
        }
    }

    [SerializeField]
    protected int[] waves;

    [SerializeField]
    protected Path path;

    [SerializeField]
    public Army playerArmy;
    [SerializeField]
    protected ParticleSystem playerParticles;

    [SerializeField]
    public Army enemyArmy;
    [SerializeField]
    protected ParticleSystem enemyParticles;

    public AnimationCurve hitCurve;

    protected float startTime = -hitInterval*2;

    int wave = 0;
    bool inCombat = false;
    bool won = false;

    public void awake() {
        wave = 0;
        playerArmy.awake(0);
        enemyArmy.awake(waves[0]);
        won = false;
        inCombat = false;
        produce = true;
    }

    public void reset() {
        StopAllCoroutines();
        wave = 0;
        playerArmy.reset(0);
        enemyArmy.reset(waves[0]);
        won = false;
        inCombat = false;
        produce = true;
    }

    public void win() {
        int enemiesLeft = enemyArmy;
        enemyArmy.gameObject.SetActive(false);
        enemyParticles.transform.position = enemyArmy.transform.position;
        enemyParticles.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, (short)enemiesLeft, (short)enemiesLeft) });
        enemyParticles.gameObject.SetActive(true);
        won = true;
    }

    public void update() {
        float v = 0;
        if ((startTime+hitInterval) >= Time.time) {
            float t = (Time.time - startTime) / hitInterval;
            v = hitCurve.Evaluate(t);
        }
        Shader.SetGlobalFloat("_RushHour", v);
        enemyArmy.update();
        if (!won && Army.distance(playerArmy, enemyArmy) <= 40 && !inCombat) { battle(); }
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
            yield return new WaitForSeconds(hitInterval/2f);
            Int2 values = new Int2(playerArmy.battleValue, enemyArmy.battleValue);
            winArmy = B.hit();
            values -= new Int2(playerArmy.battleValue, enemyArmy.battleValue);
            playerParticles.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, (short)values.x, (short)values.x) });
            enemyParticles.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, (short)values.y, (short)values.y) });
            playerParticles.gameObject.SetActive(true);
            enemyParticles.gameObject.SetActive(true);
            if (winArmy != 0) {
                ongoing = false;
                if (winArmy != -1) { wave++; enemyArmy.reset(waves[wave]); }
            }
            yield return new WaitForSeconds(hitInterval / 2f);
            playerParticles.gameObject.SetActive(false);
            enemyParticles.gameObject.SetActive(false);
        }
        if (winArmy == -1) { winner(enemyArmy); }
        else if (winArmy == 1) { winner(null); }
        else { winner(playerArmy); }
        yield return new WaitForSeconds(hitInterval / 2f);
    }

    protected void winner(Army A) {
        if (A != enemyArmy) {
            inCombat = false;
            enemyArmy.inCombat = false;
            produce = true;
        }
        else { GameManager.instance.gameOver(); }
    }
}
