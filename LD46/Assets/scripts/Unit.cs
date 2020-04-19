using UnityEngine;
using UnityEditor;

public enum Emotion {Neutral, Attack}

public class Unit : MonoBehaviour {

    [SerializeField]
    public int battleValue;

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
        changeEmotion(Emotion.Neutral);
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
