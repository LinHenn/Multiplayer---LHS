using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring { get; set; }

    public ParticleSystem fireParticleSystem;
    public Transform aimPoint;
    public LayerMask collisionLayers;

    float lastTimeFired = 0;

    HPHandler hpHandler;

    public pRegistry pScore;

    private void Awake()
    {
        hpHandler = GetComponent<HPHandler>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (hpHandler.isDead) return;

        if(GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFireButtonPressed) Fire(networkInputData.aimForwardVector);
        }
    }


    void Fire(Vector3 aimForwardVector)
    {
        if (Time.time - lastTimeFired < 0.15f) return;

        StartCoroutine(FireEffectCO());

        Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100, Object.InputAuthority, out var hitinfo, collisionLayers, HitOptions.IncludePhysX);

        float hitDistance = 100;
        bool isHitOtherPlayer = false;

        if(hitinfo.Distance > 0)
        {
            hitDistance = hitinfo.Distance;
        }

        if(hitinfo.Hitbox != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit hitbox {hitinfo.Hitbox.transform.root.name}");

            if (Object.HasStateAuthority) hitinfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage();
            isHitOtherPlayer = true;


            //scoreCmd.instance.setPoint(gameObject.name);
            pScore.score += 10;
            pScore.pScore.text = $"Score: {pScore.score}";


        }
        else if(hitinfo.Collider != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit PhysX collider {hitinfo.Collider.transform.root.name}");
        }

        //debug
        if (isHitOtherPlayer) Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 1);
        else Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 1);

        lastTimeFired = Time.time;
    }

    IEnumerator FireEffectCO()
    {
        isFiring = true;
        fireParticleSystem.Play();

        yield return new WaitForSeconds(0.09f);
        isFiring = false;
    }


    static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        //Debug.Log($"{Time.time} OnFireChanged value {changed.Behaviour.isFiring}");

        bool isFiringCurrent = changed.Behaviour.isFiring;

        //load the old value
        changed.LoadOld();

        bool isFiringOld = changed.Behaviour.isFiring;

        if(isFiringCurrent && !isFiringOld)
        {
            changed.Behaviour.OnFireRemote();
        }
    }


    void OnFireRemote()
    {
        if (!Object.HasInputAuthority) fireParticleSystem.Play();
    }
}
