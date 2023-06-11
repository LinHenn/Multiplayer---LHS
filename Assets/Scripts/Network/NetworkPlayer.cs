using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//fusion
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    public Transform playerModel;

    void Start()
    {
        
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            //sets the layer of the local players model
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            //disable main camera
            Camera.main.gameObject.SetActive(false);

            Debug.Log("Spawned local player");
        }
        else
        {
            //Disable the camera if we are not the local player
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            //Only 1 adio listner is allowed in the scene so disable remote players audio listner
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;


            Debug.Log("Spawned remote player");
        }

        //make it easier to tell wich player is which
        transform.name = $"P_{Object.name}";

    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }

}
