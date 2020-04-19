using UnityEngine;

public delegate void ValueChange(int delta);

public class Income {

    public event ValueChange income = delegate { };

    public int value;
    public float interval;
    public int increment;
    private float nextUpdate;

    public Income() {
        value = 0;
        interval = 0;
        increment = 0;
        nextUpdate = Time.time;
    }

    public Income(int increment, float interval) {
        value = 0;
        this.interval = interval;
        this.increment = increment;
        nextUpdate = Time.time;
    }

    public void update() {
        if (Time.time >= nextUpdate) {
            value += increment;
            income(increment);
            nextUpdate = Time.time + interval;
        }
    }
}
