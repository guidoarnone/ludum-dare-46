using UnityEngine;

public enum Weapon { Melee, Ranged }

public class WeaponManager : MonoBehaviour {

    public Texture2D[] weapons;
    public Texture2D[] rangedWapons;

    public Texture2D getWeapon(int tier, Weapon weapon) {
        tier = Math.clamp(tier, 0, Math.min(weapons.Length, rangedWapons.Length));
        switch (weapon) {
            case Weapon.Ranged:
                return rangedWapons[tier];
            default:
                return weapons[tier];
        }
    }
}
