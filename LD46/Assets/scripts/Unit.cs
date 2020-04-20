using UnityEngine;
using UnityEditor;

public enum Emotion {Neutral, Attack, Hit, Joy, Dead}

public class Unit : MonoBehaviour {

    [SerializeField]
    public int battleValue;

    [SerializeField]
    public Texture2D[] emotions;

    protected Material face;
    protected Material weapon;

    //TODO
    private int tier { get { return 0; } }

    public void awake(Weapon weapon = Weapon.Melee) {
        MeshRenderer MR = GetComponent<MeshRenderer>();
        if (MR.materials.Length >= 3) { face = MR.materials[2]; } else { face = MR.materials[1]; }
        if (MR.materials.Length >= 3) { this.weapon = MR.materials[3]; }
        changeEmotion(Emotion.Neutral);
        changeWeapon(weapon);
    }

    public void update() {
        
    }

    public void changeEmotion(Emotion emotion) {
        if (!face) { return; }
        face.mainTexture = emotions[(int)emotion%emotions.Length];
    }

    public void changeWeapon(Weapon weapon) {
        if (!this.weapon) { return; }
        this.weapon.mainTexture = GameManager.instance.weaponManager.getWeapon(tier, weapon);
    }
}
