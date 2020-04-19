using UnityEngine;

public class IncomeManager {

    public int seeds { get { return seedIncome.value; } }
    public int generation { get { return battleIncome.value; } }

    public Income battleIncome { get; protected set; }
    public Income seedIncome { get; protected set; }

    public IncomeManager() {
        battleIncome = new Income(1, 0.1f);
        seedIncome = new Income(5, 1);
    }

    public void update() {
        battleIncome.update();
        seedIncome.update();
    }
}
