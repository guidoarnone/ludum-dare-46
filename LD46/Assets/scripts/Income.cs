using UnityEngine;

public class Income {

  public int value;
  public int interval;
  public int increment;
  private float nextUpdate;

  public Income() {
    value = 0;
    interval = 0;
    increment = 0;
    nextUpdate = Time.time;
  }

  public void update() {
    if (Time.time >= nextUpdate) {
      value += increment;
      nextUpdate = Time.time + (float)interval;
    }
  }

}
