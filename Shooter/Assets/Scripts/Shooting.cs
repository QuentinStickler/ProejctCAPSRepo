using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Shooting : NetworkBehaviour
{
    public new Camera camera;
    public ParticleSystem muzzleFlash;
    public ParticleSystem bloodEffect;

    public float damage = 20f;
    public float fireRate = 15f;
    float nextTimetoFire = 0f;

    public int maxAmmo;
    public int currentAmmo;
    public int reserveAmmo;
    public int current;
    //  public int current;

    public new AudioSource audio;

    public float reloadTime = 1f;
    public float aimSpeed;

    public GameObject hitmarker;
    public GameObject crossHair;

    private bool isReloading = false;
    public bool isAutomatic;

    public Animator animator;

    public Text ammoDisplay;
    public Text reserveAmmoDisplay;

    public Vector3 hipFire;
    public Vector3 aimDownSight;


    private void Start()
    {
        currentAmmo = maxAmmo;
        hitmarker.SetActive(false);
        audio = camera.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //Alles deaktivieren, damit beim Waffenwechsel nicht die ganzen Sachen kommen
        isReloading = false;
        //animator.SetBool("isReloading", false);
        hitmarker.SetActive(false);

    }
    void Shoot()
    {
            muzzleFlash.Play();
            currentAmmo--;
            RaycastHit hit;
            //Shoot out a raycast to the middle with infinite range
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit))
            {
                Debug.DrawLine(camera.transform.position, hit.point, Color.red);
                if (!(hit.transform.tag == "Enemy"))
                {
                    return;
                }
                //Get the script of the enemy
                EnemyBehaviour target = hit.transform.GetComponent<EnemyBehaviour>();
                if (target != null)
                {
                Debug.Log("YOu hit an enemy");
                    audio.Play();
                    target.TakeDamage(damage);
                    hitmarker.SetActive(true);
                    //Hitmarker despawnt nach 0.2 Sekunden wieder
                    Invoke("HitDisable", 0.15f);
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
    }

    IEnumerator Reload()
    {if (reserveAmmo > 0)
        {
            isReloading = true;
           // animator.SetBool("isReloading", true);
            yield return new WaitForSeconds(reloadTime - .25f); 
            //animator.SetBool("isReloading", false);
            yield return new WaitForSeconds(.25f);
            //Entweder man hat noch genug Munition als Reserve(Mehr als maximale Kapazität)
            if (reserveAmmo >= maxAmmo)
            {
                current = maxAmmo - currentAmmo;
                currentAmmo = maxAmmo;
                reserveAmmo -= current;
            }
            //Oder man hat weniger als die maximale Kapazität als Reserve aber man hat noch viel im Magazin (Z.B. 25,25)
            else if (currentAmmo + reserveAmmo > maxAmmo)
            {
                current = maxAmmo - currentAmmo;
                currentAmmo = maxAmmo;
                reserveAmmo -= current;
                current = 0;
            }
            //Oder man hat nicht mehr genug, um auf die maximale Kapazität zu kommen
            else
            {
                currentAmmo += reserveAmmo;
                reserveAmmo = 0;
            }
            isReloading = false;
            current = 0;
        }
    }
    void Update()
    {
        
        ammoDisplay.text = currentAmmo.ToString();
        reserveAmmoDisplay.text = reserveAmmo.ToString();
        if (isReloading) { return; }
        if ((currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R)) && currentAmmo != maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }
        if (!isAutomatic)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else if (Input.GetButton("Fire1") && Time.time >= nextTimetoFire)
        {
            nextTimetoFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (Input.GetMouseButton(1))
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, aimDownSight, aimSpeed * Time.deltaTime);
            crossHair.SetActive(false);
        }
        else
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, hipFire, aimSpeed * Time.deltaTime);
            crossHair.SetActive(true);
        }
    }

    void HitDisable()
    {
        hitmarker.SetActive(false);
    }
}