using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon;

    [SerializeField]
    private PlayerWeapon secondaryWeapon;

    [SerializeField]
    private Transform WeaponHolder;

    [SerializeField]
    private Transform primaryHolder;

    [SerializeField]
    private Transform secondaryHolder;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;


    public Camera gunCamera;
    public Camera playerCamera;

    private int current;

    public bool isReloading;
    Animator anim;

    FirstPersonController controller;

    PlayerShoot shoot;

    public bool crossHairEnabled;

    public int selectedWeapon = 0;

    //public int selectedWeapon = 0;

    private PlayerWeapon[] guns = new PlayerWeapon[2];
    private GameObject[] gunHolders = new GameObject[2];

    private void Start()
    {
        guns[0] = primaryWeapon;
        guns[1] = secondaryWeapon;
        gunHolders[0] = primaryHolder.gameObject;
        gunHolders[1] = secondaryHolder.gameObject;
        //SelectWeapon();
        shoot = GetComponent<PlayerShoot>();
        gunCamera.fieldOfView = 50;
        playerCamera.fieldOfView = 90;
        controller = GetComponent<FirstPersonController>();
        EquipWeapon(secondaryWeapon);
        EquipWeapon(primaryWeapon);
        primaryHolder.gameObject.SetActive(true);
        secondaryHolder.gameObject.SetActive(false);
        //currentWeapon = primaryWeapon;
        //anim = currentGraphics.GetComponent<Animator>();
    }
    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public void SetCurrentGraphics(WeaponGraphics graphics)
    {
        currentGraphics = graphics;
    }

    public bool getCrossHair()
    {
        return crossHairEnabled;
    }
    void EquipWeapon(PlayerWeapon weapon)
    {
        currentWeapon = weapon;

        if (weapon == primaryWeapon)
        {
            GameObject weaponInstance = Instantiate(weapon.graphics, primaryHolder.position, primaryHolder.rotation);
            weaponInstance.transform.SetParent(primaryHolder);

            currentGraphics = weaponInstance.GetComponent<WeaponGraphics>();

            if (currentGraphics == null)
            {
                Debug.Log("No Weapon Graphics on gun");
            }

            if (isLocalPlayer)
            {
                //Set die Layer von der Waffe und all seinen children zu dem angegegebenen Layer mit der rekursiven method in Util->
                //Dadurch werden die Waffenobjekte richtig gerendert von der Waffenkamera
                Util.SetlayerRecursively(weaponInstance, LayerMask.NameToLayer("Weapon"));
            }
        }
        else if (weapon == secondaryWeapon)
        {
            GameObject weaponInstance = Instantiate(weapon.graphics, secondaryHolder.position, secondaryHolder.rotation * Quaternion.Euler(0f,-90f,0f));
            weaponInstance.transform.SetParent(secondaryHolder);

            currentGraphics = weaponInstance.GetComponent<WeaponGraphics>();

            if (currentGraphics == null)
            {
                Debug.Log("No Weapon Graphics on gun");
            }


            if (isLocalPlayer)
            {
                //Set die Layer von der Waffe und all seinen children zu dem angegegebenen Layer mit der rekursiven method in Util->
                //Dadurch werden die Waffenobjekte richtig gerendert von der Waffenkamera
                Util.SetlayerRecursively(weaponInstance, LayerMask.NameToLayer("Weapon"));
            }
        }

    }

    public void Reload()
    {
        if (isReloading)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = primaryWeapon;
            primaryHolder.gameObject.SetActive(true);
            secondaryHolder.gameObject.SetActive(false);
            currentGraphics = primaryHolder.GetComponentInChildren<WeaponGraphics>();
            //SetCurrentGraphics(currentWeapon.graphics);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = secondaryWeapon;
            primaryHolder.gameObject.SetActive(false);
            secondaryHolder.gameObject.SetActive(true);
            currentGraphics = secondaryHolder.GetComponentInChildren<WeaponGraphics>();
        }
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= guns.Length - 1)
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
                selectedWeapon = guns.Length - 1;
            }
            else
                selectedWeapon--;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            CmdSelectWeapon();
        }
        anim = GetCurrentGraphics().GetComponent<Animator>();

        if (Input.GetMouseButton(1))
        {
            WeaponHolder.localPosition = Vector3.Slerp(WeaponHolder.localPosition, currentWeapon.aimDownSight, currentWeapon.aimSpeed * Time.deltaTime);
            gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView, 40, currentWeapon.aimSpeed * Time.deltaTime);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 70, currentWeapon.aimSpeed * Time.deltaTime);
            crossHairEnabled = false;
            controller.SetAiming(true);
           // crossHair.GetComponent<Canvas>().enabled = false;
            shoot.SetisAiming(true);
            CmdOnAim();
        }
        else
        {
            WeaponHolder.localPosition = Vector3.Slerp(WeaponHolder.localPosition, currentWeapon.hipFire, currentWeapon.aimSpeed * Time.deltaTime);
            gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView, 50, currentWeapon.aimSpeed * Time.deltaTime);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 90, currentWeapon.aimSpeed * Time.deltaTime);
            controller.SetAiming(false);
            crossHairEnabled = true;
            //crossHair.GetComponent<Canvas>().enabled = true;
            shoot.SetisAiming(false);
            CmdOnUnAim();
        }
    }

    IEnumerator ReloadCoroutine()
    {
        if (currentWeapon.reserveAmmo > 0)
        {
            isReloading = true;
            CmdOnReload();
            yield return new WaitForSeconds(currentWeapon.reloadTime - .25f);
            yield return new WaitForSeconds(.25f);
            //Entweder man hat noch genug Munition als Reserve(Mehr als maximale Kapazität)
            if (currentWeapon.reserveAmmo >= currentWeapon.maxAmmo)
            {
                current = currentWeapon.maxAmmo - currentWeapon.currentAmmo;
                currentWeapon.currentAmmo = currentWeapon.maxAmmo;
                currentWeapon.reserveAmmo -= current;
            }
            //Oder man hat weniger als die maximale Kapazität als Reserve aber man hat noch viel im Magazin (Z.B. 25,25)
            else if (currentWeapon.currentAmmo + currentWeapon.reserveAmmo > currentWeapon.maxAmmo)
            {
                current = currentWeapon.maxAmmo - currentWeapon.currentAmmo;
                currentWeapon.currentAmmo = currentWeapon.maxAmmo;
                currentWeapon.reserveAmmo -= current;
                current = 0;
            }
            //Oder man hat nicht mehr genug, um auf die maximale Kapazität zu kommen
            else
            {
                currentWeapon.currentAmmo += currentWeapon.reserveAmmo;
                currentWeapon.reserveAmmo = 0;
            }
            isReloading = false;
            current = 0;
        }
    }

    [Command]
    void CmdSelectWeapon()
    {
        RpcSelectWeapon();
    }
 
    [Command]
    void CmdOnAim() { 
        RpcOnAim();
    }

    [ClientRpc]
    void RpcSelectWeapon()
    {
        int i = 0;
        foreach (PlayerWeapon equipWeapons in guns)
        {
            if (i == selectedWeapon)
            {
                gunHolders[i].SetActive(true);
                currentWeapon = guns[selectedWeapon];//geht nich wegen Monobehaviour?????????
                currentGraphics = gunHolders[i].GetComponentInChildren<WeaponGraphics>();
                // currentWeapon = equipWeapons[selectedWeapon].ga;
                // currentGraphics = primaryHolder.GetComponentInChildren<WeaponGraphics>();
            }
            else
                gunHolders[i].SetActive(false);
            //currentWeapon = guns[selectedWeapon].GetComponentInChildren<PlayerWeapon>();
            //currentGraphics = guns[selectedWeapon].GetComponentInChildren<WeaponGraphics>();
            i++;
        }
    }

    [ClientRpc]
    void RpcOnAim()
    {
        if (anim != null)
            anim.SetBool("isAiming", true);
    }

    [Command]
    void CmdOnUnAim()
    {
        RpcOnUnAim();
    }

    [ClientRpc]
    void RpcOnUnAim()
    {
        if (anim != null)
            anim.SetBool("isAiming", false);
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {

        if(anim != null)
        {
            anim.SetTrigger("Reload");
        }
    }
}  
