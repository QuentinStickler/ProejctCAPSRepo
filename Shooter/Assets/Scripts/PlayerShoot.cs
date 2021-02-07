using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";


    private PlayerWeapon currentWeapon;
    public Camera camera;

    private WeaponManager manager;

    float nextTimetoFire = 0f;
    public float bulletSpreadFactor;

    public int extraScore;

    public bool isAiming;
    Vector3 shotDestination;

    public bool hitmarkerEnabled;
    public bool killHitmarkerEnabled;
    public bool headShotHitmarker;
    PlayerStats stats;

    public AudioClip assaultRifleShot;
    public AudioClip pistoleShot;
    private FirstPersonController controller;
    private void Start()
    {
        extraScore = 0;
        isAiming = false;
        manager = GetComponent<WeaponManager>();
        stats = GetComponent<PlayerStats>();
        controller = GetComponent<FirstPersonController>();
    }

    private void OnEnable()
    {
        //Alles deaktivieren, damit beim Waffenwechsel nicht die ganzen Sachen kommen
        //animator.SetBool("isReloading", false);
    }

    public bool getHitmarker()
    {
        return hitmarkerEnabled;
    }
    public bool getKillHitmarker()
    {
        return killHitmarkerEnabled;
    }
    public bool getHeadShotKillHitmarker()
    {
        return headShotHitmarker;
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    void Update()
    {
        currentWeapon = manager.GetCurrentWeapon();

        if ((currentWeapon.currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R)) && currentWeapon.currentAmmo != currentWeapon.maxAmmo)
        {
            manager.Reload();
            return;
        }
        if (!currentWeapon.isAutomatic)
        {
            if (Input.GetButtonDown("Fire1"))
            {       
                Shoot();
                controller.isShooting = true;
            }
            controller.isShooting = false;
        }
        else if (Input.GetButton("Fire1") && Time.time >= nextTimetoFire)
        {
            nextTimetoFire = Time.time + 1f / currentWeapon.fireRate;
            Shoot();
            controller.isShooting = true;
        }
        controller.isShooting = false;
    }

    public void SetisAiming(bool aiming)
    {
        isAiming = aiming;
    }

    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();

    }

    [Command]
    void CmdOnHit(string name, Vector3 pos, Vector3 normal)
    {
        //Allen Clients sagen, dass sie den Effect playen sollen
        DoHitEffect(name, pos, normal);

    }

    [ClientRpc]
    void DoHitEffect(string name, Vector3 pos, Vector3 normal)
    {
        if (name.Equals("Enemy") || name.Equals("Player"))
        {
            GameObject hitObject = Instantiate(manager.GetCurrentGraphics().bloodEffectPrefab, pos, Quaternion.LookRotation(normal));
            Destroy(hitObject, 0.9f);
        }
        else
        {
            GameObject bulletHole = Instantiate(manager.GetCurrentGraphics().bulletHole, pos, Quaternion.LookRotation(-normal));
            GameObject hitObject = Instantiate(manager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
            Destroy(hitObject, 0.9f);
            Destroy(bulletHole, 3f);
        }
    }
    [ClientRpc]
    void RpcDoShootEffect()
    {
        manager.GetCurrentGraphics().muzzleFlash.Play();
        if (currentWeapon.id == "M4") { AudioSource.PlayClipAtPoint(assaultRifleShot, transform.position, 0.05f); }
        else if (currentWeapon.id == "Glock") { AudioSource.PlayClipAtPoint(pistoleShot, transform.position, 0.1f); }
    }

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer) { return; }
        if (manager.isReloading) { return; }
        CmdOnShoot();
        currentWeapon.currentAmmo--;
        RaycastHit hit;
        if (isAiming) { shotDestination = camera.transform.forward; }
        else
        {
            shotDestination = camera.transform.forward;
            shotDestination.y += Random.Range(-bulletSpreadFactor, bulletSpreadFactor);
            shotDestination.x += Random.Range(-bulletSpreadFactor, bulletSpreadFactor);
        }
        if (Physics.Raycast(camera.transform.position, shotDestination, out hit))
        {
            if (hit.collider.CompareTag(PLAYER_TAG))
            {
                //Debug.DrawLine(camera.transform.position, hit.point, Color.red, 2f);
                CmdShoot(hit.collider.name, currentWeapon.damage);
                CmdOnHit("Player", hit.point, hit.normal);                
                hitmarkerEnabled = true;
                // ui.GetComponent<PlayerUi>().SetHitmarker(true);
                //hitSound.Play();
                Invoke(nameof(DisableHitmarker), 0.15f);
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                //Debug.DrawLine(camera.transform.position, hit.point, Color.red, 2f);
                CmdOnHit("Enemy", hit.point, hit.normal);
                if (hit.collider is BoxCollider)
                {
                    headShotHitmarker = true;
                    EnemyShoot(hit.collider.name, currentWeapon.headShotDamage, true);
                    Invoke(nameof(DisableHeadShotHitmarker), 0.25f);
                }
                else 
                hitmarkerEnabled = true;
                EnemyShoot(hit.collider.name, currentWeapon.damage,false);
                Invoke(nameof(DisableHitmarker), 0.15f);
            }
            else CmdOnHit("Floor", hit.point, hit.normal);
        }
    }

    void DisableHitmarker()
    {
        hitmarkerEnabled = false;
    }
    void DisableKillHitmarker()
    {
        killHitmarkerEnabled = false;
    }
    void DisableHeadShotHitmarker()
    {
        headShotHitmarker = false;
    }
    [Command]
    void CmdShoot(string id, float damage)
    {
        Player thisPlayer = GameManager.GetPlayer(GetComponent<NetworkIdentity>().name);
        Player player = GameManager.GetPlayer(id);
        player.RpcTakeDamage(damage, thisPlayer);
    }

    void EnemyShoot(string name, float damage, bool isHeadShot)
    {
        if (isHeadShot)
        {
            extraScore = 25;
        }
        GameObject enemy = GameObject.Find(name);
        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        enemyBehaviour.TakeDamage(damage);
        if(enemyBehaviour.isDead)
        {
            stats.score += 100 + extraScore;
            killHitmarkerEnabled = true;
            Invoke(nameof(DisableKillHitmarker), 0.25f);
            extraScore = 0;
        }
    }

    //asihsauh

}