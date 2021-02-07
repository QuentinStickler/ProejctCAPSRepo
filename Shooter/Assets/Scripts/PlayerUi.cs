using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    [SerializeField]
    RectTransform healthBarFill;

    [SerializeField]
    private Text currentAmmoText;
    [SerializeField]
    private Text reserveAmmoText;
    [SerializeField]
    private Canvas hitmarker;
    [SerializeField]
    private Canvas killHitmarker;
    [SerializeField]
    private Canvas headShotHitmarker;
    [SerializeField]
    private Canvas crossHair;
    [SerializeField]
    private Text score;

    private Player player;
    private FirstPersonController controller;
    private WeaponManager manager;
    private PlayerShoot shooter;
    private PlayerStats stats;

    private void Start()
    {
       // hitmarker.enabled = false;
    }
    public void SetPlayer(Player _player)
    {
        player = _player;
        manager = player.GetComponent<WeaponManager>();
        controller = player.GetComponent<FirstPersonController>();
        shooter = player.GetComponent<PlayerShoot>();
        stats = player.GetComponent<PlayerStats>();
    }
    void SetHealthAmmount(float amount)
    {
        healthBarFill.localScale = new Vector3(amount, 1f, 1f);
    }

    void SetCurrentAmmo(int amount)
    {
        currentAmmoText.text = amount.ToString();
    }
    void SetReserveAmmo(int amount)
    {
        reserveAmmoText.text = amount.ToString();
    }
    public void SetHitmarker(bool enabled)
    {
        hitmarker.enabled = enabled;
    }
    public void SetHeadShotHitmarker(bool enabled)
    {
        headShotHitmarker.enabled = enabled;
    }
    public void SetKillHitmarker(bool enabled)
    {
        killHitmarker.enabled = enabled;
    }
    public void SetCrossHair(bool enabled)
    {
        crossHair.enabled = enabled;
    }
    public void SetScore(int _score)
    {
        score.text = _score.ToString();
    }
    private void Update()
    {
        SetCrossHair(manager.getCrossHair());
        SetHitmarker(shooter.getHitmarker());
        SetHealthAmmount(player.getHealthPercentage());
        SetCurrentAmmo(manager.GetCurrentWeapon().currentAmmo);
        SetReserveAmmo(manager.GetCurrentWeapon().reserveAmmo);
        SetScore(stats.score);
        SetKillHitmarker(shooter.getKillHitmarker());
        SetHeadShotHitmarker(shooter.getHeadShotKillHitmarker());
    }
}