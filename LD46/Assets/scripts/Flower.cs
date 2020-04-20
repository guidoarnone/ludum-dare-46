using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour {

    public GameObject fire;
    public GameObject confetti;

    [SerializeField]
    float winTime;

    [Range(0, 1)]
    [SerializeField]
    float radius;

    [Range(0, 10)]
    [SerializeField]
    float smoothRadius;

    float startTime;

    protected bool dead = false;

    void Awake() {
        dead = false;
        Shader.SetGlobalVector("_ColorSource", transform.position);
        startTime = Time.time;
        radius = 0;
    }

    public void reset() {
        fire.SetActive(false);
        confetti.SetActive(false);
        Awake();
    }

    private void Update() {
        if (dead) {
            radius -= 0.25f * Time.deltaTime;
            if (radius <= -0.1) { fire.SetActive(true); }
            if (radius <= -0.5) { Time.timeScale = 0; GameManager.instance.reset(); }
        }
        else {
            radius = (Time.time-startTime) / winTime;
            if (radius >= 1) { GameManager.instance.win();  }

        }
        float r = radius * 120 == 0 ? 0.001f : radius * 120;
        Shader.SetGlobalFloat("_ColorRadius", r);
        Shader.SetGlobalFloat("_SmoothRadius", (Math.clamp01(radius)+0.001f) * 20);
    }

    public void gameOver() {
        dead = true;
    }

    public void win() {
        confetti.SetActive(true);
    }
}
