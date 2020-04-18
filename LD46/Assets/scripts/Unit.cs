using UnityEngine;
using UnityEditor;

public enum Emotion { Dead, Neutral, Angry, Joy}

public class Unit : MonoBehaviour {

    [SerializeField]
    protected int battleValue;

    [SerializeField]
    protected GameObject weapon;
    [SerializeField]
    protected GameObject face;

    [SerializeField]
    public Texture2D[] emotions;

    protected Material faceMat;
    protected Material weaponMat;

    private int tier { get { return battleValue / 25; } }

    void awake(Weapon weapon = Weapon.Melee) {
        faceMat = face.GetComponent<MeshRenderer>().material;
        weaponMat = this.weapon.GetComponent<MeshRenderer>().material;
        changeWeapon(weapon);
    }

    public void update() {
        
    }

    public void changeEmotion(Emotion emotion) {
        faceMat.mainTexture = emotions[(int)emotion%emotions.Length];
    }

    public void changeWeapon(Weapon weapon) {
        weaponMat.mainTexture = GameManager.instance.weaponManager.getWeapon(tier, weapon);
    }
}
