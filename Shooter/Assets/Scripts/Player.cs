 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get{ return _isDead; }
        protected set { _isDead = value; }
    }
    WeaponManager manager;
    [SerializeField]
    private float maxHealth = 100f;

    [SyncVar]
    private float currentHealth;

    [SerializeField]
    Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    private void Start()
    {
        manager = GetComponent<WeaponManager>();
    }
    public void Setup()
    {
        //if (!isLocalPlayer) { return; }
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }

    private void SetDefaults()
    {    
        isDead = false;
        currentHealth = maxHealth;
        if (manager != null)
        {
            manager.GetCurrentWeapon().currentAmmo = manager.GetCurrentWeapon().maxAmmo;
            manager.GetCurrentWeapon().reserveAmmo = manager.GetCurrentWeapon().reservedAmmo;
        }

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i]; //Not set to an instance of an object --> Nochma Brackex Video dazu angucken
        }

        Collider collider = GetComponent<Collider>();
        if(collider != null)
        {
            collider.enabled = true;
        }
    }
    [ClientRpc]
    public void RpcTakeDamage (float ammount, Player killedBy)
    {
        if (_isDead) { return; }
        currentHealth -= ammount;
        Debug.Log("ammount of damage: " + ammount);
        Debug.Log(transform.name + " now has " + currentHealth + " health");
        if(currentHealth <= 0) {
            killedBy.GetComponent<PlayerStats>().score += 100;
            Die(); }
    }

    void Die()
    {
        _isDead = true;

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        StartCoroutine(Respawn());
    }
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.settings.respawnTime);
        SetDefaults();
        Transform startPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;
        Setup();
    }

    public float getHealthPercentage()
    {
        return (float) currentHealth / maxHealth;
    }
}