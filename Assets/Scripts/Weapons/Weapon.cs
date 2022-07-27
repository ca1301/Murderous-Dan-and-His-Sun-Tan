using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
    public RuntimeAnimatorController animController;

    public Camera cam;
    public LayerMask shootableLayers;

    [Header("Weapon Stats")]
    public int weaponId;
    public string weaponName;

    public int bullets;
    public float range;

    public float headDamage = 100;
    public float bodyDamage = 50;
    public float legDamage = 20;
    public float armDamage = 10;


    [Header("Weapon Effects")]
    public GameObject muzzleFlash;
    public Transform fpMuzzle;
    public AudioClip fireSound;


    [Header("Third Person")]
    public Transform tpMuzzle;
    public Transform thirdPersonWeapon;
    public Transform leftHandTransform;
    public Transform rightHandTransform;


    public void ApplyDamage(Transform hit)
    {

            if (hit.CompareTag("Head"))
            {
                hit.GetComponentInParent<NetworkPlayer>().CmdApplyDamage(headDamage);
            }
            if (hit.CompareTag("Body"))
            {
                hit.GetComponentInParent<NetworkPlayer>().CmdApplyDamage(bodyDamage);
            }
            if (hit.CompareTag("Arm"))
            {
                hit.GetComponentInParent<NetworkPlayer>().CmdApplyDamage(armDamage);
            }
            if (hit.CompareTag("Leg"))
            {
                hit.GetComponentInParent<NetworkPlayer>().CmdApplyDamage(legDamage);
            }
    }

    public virtual void Update()
    {
        thirdPersonWeapon.transform.LookAt(cam.transform.forward * range);
    }
}
