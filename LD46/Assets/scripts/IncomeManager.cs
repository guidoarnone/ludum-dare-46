using UnityEngine;

public class IncomeManager : MonoBehaviour {

    public Army income;

    public int seeds { get { return seedIncome.value; } set { seedIncome.value = value; } }
    public int troops { get { return playerTroops.value; } }

    public Income playerTroops { get; protected set; }
    public Income enemyTroops { get; protected set; }
    public Income seedIncome { get; protected set; }

    public void awake() {
        playerTroops = new Income(1, 5f);
        enemyTroops = new Income(4, 0.1f);
        seedIncome = new Income(1, 1);

        income.awake(1);
        seedIncome.incrementChange += income.add;
    }

    public void reset() {
        playerTroops.increment = 1;
        playerTroops.interval = 5f;

        enemyTroops.increment = 4;
        enemyTroops.interval = 0.1f;

        seedIncome.value = 0;
        seedIncome.increment = 1;
        seedIncome.interval = 1f;
        income.reset(1);
    }

    public void update() {
        playerTroops.update();
        enemyTroops.update();
        seedIncome.update();
    }
}
