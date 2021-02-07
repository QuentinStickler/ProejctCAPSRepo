using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string id = "M4";
    public float damage = 20f;
    public float headShotDamage = 30f;

    public float fireRate = 10f;

    public float reloadTime = 1.2f;

    public bool isAutomatic = true;

    public int currentAmmo = 30;

    public int maxAmmo = 30;

    public int reserveAmmo = 90;

    public int reservedAmmo = 90;

    public float aimSpeed;

    public Vector3 hipFire;

    public Vector3 aimDownSight;

    public GameObject graphics;

    public PlayerWeapon()
    {
        currentAmmo = maxAmmo;
    }

}
