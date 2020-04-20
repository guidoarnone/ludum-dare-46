using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Flower : MonoBehaviour {

    [SerializeField]
    float radius;

    float startTime;

    protected bool dead = false;

    void Awake() {
        dead = false;
        startTime = Time.time;
        radius = 0;
    }

    public void reset() {
        Awake();
    }

    private void Update() {
        if (dead) {
            radius -= 0.1f * Time.deltaTime;
            if (radius <= -0.5) { Time.timeScale = 0; GameManager.instance.reset(); }
        }
        else { radius = (Time.time-startTime) / 360f; }
        Shader.SetGlobalFloat("_ColorRadius", radius * 120);
    }

    public void gameOver() {
        dead = true;
    }
}
