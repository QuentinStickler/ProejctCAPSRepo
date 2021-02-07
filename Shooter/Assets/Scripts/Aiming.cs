using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
    public Vector3 hipFire;
    public Vector3 m4AimDownSight;
    public Vector3 pistolAimDownSight;

    public int currentWeapon;

    void Update()
    {
        Aim();
    }
    void Aim()
    {
            if (Input.GetMouseButton(1))
            {
                if (currentWeapon == 0)
                {
                    transform.position = m4AimDownSight;
                }
                else if (currentWeapon == 1) transform.position = pistolAimDownSight;
            }
            transform.position = hipFire;
    }
    public int getCurrentWeapon()
    {
        return currentWeapon;
    }

    public void setCurrentWeapon(int gun)
    {
        currentWeapon = gun;
    }
}
