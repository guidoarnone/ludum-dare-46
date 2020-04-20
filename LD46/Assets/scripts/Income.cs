using UnityEngine;

public delegate void ValueChange(int delta);

public class Income {

    public event ValueChange valueChange = delegate { };
    public event ValueChange intervalChange = delegate { };
    public event ValueChange incrementChange = delegate { };

    public int value { get { return _value; } set { valueChange(value - _value); _value = value; } }
    protected int _value;
    public float interval;
    public int increment { get { return _increment; } set { incrementChange(value-_increment); _increment = value; } }
    protected int _increment;

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
            nextUpdate = Time.time + interval;
        }
    }
}
