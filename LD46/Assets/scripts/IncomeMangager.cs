using UnityEngine;

public class IncomeManager {

    public int seeds { get { return seedIncome.value; } }
    public int troops { get { return playerTroops.value; } }

    public Income playerTroops { get; protected set; }
    public Income enemyTroops { get; protected set; }
    public Income seedIncome { get; protected set; }

    public IncomeManager() {
        playerTroops = new Income(5, 0.1f);
        enemyTroops = new Income(4, 0.1f);
        seedIncome = new Income(5, 1);
    }

    public void reset() {
        playerTroops.increment = 5;
        playerTroops.interval = 0.1f;
        enemyTroops.increment = 4;
        enemyTroops.interval = 0.1f;
        seedIncome.increment = 5;
        seedIncome.interval = 1f;
    }

    public void update() {
        playerTroops.update();
        enemyTroops.update();
        seedIncome.update();
    }
}
