using UnityEngine;

public class IncomeManager {

  public int seeds { get { return seedIncome.value; } }
  public int generation { get { return battleIncome.value; } }

  private Income battleIncome;
  private Income seedIncome;

  public IncomeManager() {
    battleIncome = new Income();
    seedIncome = new Income();
  }

  public void update() {
    battleIncome.update();
    seedIncome.update();
  }

  public void setBattleIncrement(int i) {
    battleIncome.increment = i;
  }

  public void setBattleInterval(int i) {
    battleIncome.interval = i;
  }

  public void setSeedIncrement(int i) {
    seedIncome.increment = i;
  }

  public void setSeedInterval(int i) {
    seedIncome.interval = i;
  }

}
