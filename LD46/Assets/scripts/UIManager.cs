using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {

    public TextMeshProUGUI seeds;
    public TextMeshProUGUI units;

    public Text seedInterval;
    public Text seedIncrement;
    public Text troopInterval;
    public Text troopIncrement;

    protected IncomeManager incomeManager;
    protected int seedIncrementCost;
    protected int seedIntervalCost;
    protected int troopIncrementCost;
    protected int troopIntervalCost;

    public void awake() {
        incomeManager = GameManager.instance.incomeManager;
        seedIncrementCost = 10;
        seedIntervalCost = 100;
        troopIncrementCost = 5;
        troopIntervalCost = 100;
    }

    public void reset() {
        awake();
    }


    public void upgradeSeedIncrement() {
        if (pay(seedIncrementCost)) {
            incomeManager.seedIncome.increment++;
            seedIncrementCost *= 2;
            update();
        }
    }

    public void upgradeSeedInterval() {
        if (pay(seedIntervalCost)) {
            incomeManager.seedIncome.interval /= 2;
            seedIntervalCost *= 10;
            update();
        }
    }

    public void upgradeTroopIncrement() {
        if (pay(troopIncrementCost)) {
            incomeManager.playerTroops.increment++;
            troopIncrementCost += Math.max(Math.round(Mathf.Log10(troopIncrementCost)) * 10, 10);
            update();
        }
    }

    public void upgradeTroopInterval() {
        if (pay(troopIntervalCost)) {
            incomeManager.playerTroops.interval /= 2;
            troopIntervalCost *= 10;
            update();
        }
    }

    protected bool pay(int payment) {
        if (incomeManager.seedIncome.value >= payment) {
            incomeManager.seedIncome.value -= payment;
            return true;
        }
        return false;
    }

    public void seed() {
        incomeManager.seedIncome.value += 1;
    }

    public void update() {
        seeds.text = incomeManager.seeds.ToString() + " + " + incomeManager.seedIncome.increment.ToString() + "/" + incomeManager.seedIncome.interval.ToString() + "s";
        units.text = GameManager.instance.combatManager.playerArmy.battleValue.ToString() + " + " + incomeManager.playerTroops.increment.ToString() + "/" + incomeManager.playerTroops.interval.ToString() + "s";
        seedIncrement.text = "Upgrade increment: " + seedIncrementCost;
        seedInterval.text = "Upgrade interval: " + seedIntervalCost;
        troopIncrement.text = "Upgrade increment: " + troopIncrementCost;
        troopInterval.text = "Upgrade interval: " + troopIntervalCost;
    }
}
