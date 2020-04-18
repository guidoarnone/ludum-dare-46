using UnityEngine;
using UnityEditor;

public class Unit : MonoBehaviour {

    public Material weaponTexture;

    private int tier { get { return battleValue / 25; } }
    protected int battleValue;

    void awake(Weapon weapon) {
        weaponTexture.mainTexture = GameManager.instance.weaponManager.getWeapon(tier, weapon); 
    }
}
