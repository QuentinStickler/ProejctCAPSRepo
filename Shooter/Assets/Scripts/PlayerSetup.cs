using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
            gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
        }
        else
        {
            playerUIInstance = Instantiate(playerUIPrefab);
            //playerUIInstance.GetComponent<Canvas>().enabled = true;
            playerUIInstance.name = playerUIPrefab.name;

            PlayerUi ui = playerUIInstance.GetComponent<PlayerUi>();
            if (ui == null)
            {
                Debug.Log("Yo there is no Ui here maaaaaaan");
            }
            ui.SetPlayer(GetComponent<Player>());
            GetComponent<Player>().Setup();
        }
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netId, _player);
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);
        GameManager.UnRegisterPlayer(transform.name);

    }
}
  