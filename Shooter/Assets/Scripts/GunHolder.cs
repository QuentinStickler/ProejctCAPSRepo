using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GunHolder : NetworkBehaviour
{
    public int selectedWeapon = 1;
    public GameObject m4;
    public GameObject pistol;
    private GameObject m4Clone;
    private GameObject pistolClone;
    public GameObject gunPosition;
    Quaternion m4rotation = Quaternion.Euler(0, -180f, 0);
    Quaternion pistolRotation = Quaternion.Euler(0, 90f, 0);
    Vector3 offsetPosition = new Vector3(1f, 0, 0);
    public Transform lookAt;
    public Transform lookAtFurther;

    private GameObject[] weapons = new GameObject[2];
    void Start()
    {
            m4Clone = Instantiate(m4, transform.position + offsetPosition, Quaternion.identity * m4rotation);
            pistolClone = Instantiate(pistol, transform.position, Quaternion.identity * pistolRotation);
            weapons[0] = m4Clone;
            weapons[1] = pistolClone;
            SelectWeapon();    
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            m4Clone.transform.position = lookAt.transform.position;
            m4Clone.transform.LookAt(lookAtFurther);
            m4Clone.transform.rotation = lookAt.rotation * m4rotation;

            pistolClone.transform.position = lookAt.transform.position;
            pistolClone.transform.LookAt(lookAtFurther);
            pistolClone.transform.rotation = lookAt.rotation * pistolRotation;
            int previousWeapon = selectedWeapon;
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedWeapon >= weapons.Length - 1)
                {
                    selectedWeapon = 0;
                }
                else
                    selectedWeapon++;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (selectedWeapon <= 0)
                {
                    selectedWeapon = weapons.Length - 1;
                }
                else
                    selectedWeapon--;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedWeapon = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedWeapon = 1;
            }

            if (previousWeapon != selectedWeapon)
                SelectWeapon();
        }
    }

    public void SelectWeapon()
    {
        for (int i = 0; i<weapons.Length; i++)
        {
            if(i == selectedWeapon)
            {
                weapons[i].gameObject.SetActive(true);
            }
            else weapons[i].gameObject.SetActive(false);
        }
    }

}
