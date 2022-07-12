using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Pistol : Weapon
{
    public float fireRate;
    float lastBullet;

    public int bullets;
    public int weaponId;

    public GameObject muzzleFlash;
    public Transform fpMuzzle;
    public Transform tpMuzzle;
    public AudioClip fireSound;
    public Camera cam;
    public LayerMask shootableLayers;
    public GameObject tracer;
    [HideInInspector]
    public bool fullAuto;

    public void Update()
    {
        if (Input.GetMouseButton(0) && bullets > 0 && Time.time > fireRate + lastBullet)
        {
            lastBullet = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        armsAnim.SetBool("Fire", true);
        StartCoroutine(EndFire(fireRate));
        GetComponentInParent<PhotonView>().RPC("CreateMuzzleFlash", RpcTarget.Others, weaponId, tpMuzzle.position, tpMuzzle.rotation);
        var muz = Instantiate(muzzleFlash, fpMuzzle.position, fpMuzzle.rotation);
        AudioSource.PlayClipAtPoint(fireSound, fpMuzzle.position);
        Destroy(muz, 1);
        RaycastHit hit;
        Vector3 origin = cam.transform.position + cam.transform.forward * 1;
        if(Physics.Raycast(origin, cam.transform.forward, out hit, range, shootableLayers))
        {
            GetComponentInParent<PhotonView>().RPC("CreateTracer", RpcTarget.Others, weaponId, tpMuzzle.position, hit.point);
            var trace = Instantiate(tracer, fpMuzzle.position, fpMuzzle.rotation);
            trace.GetComponent<Tracer>().target = hit.point;
            trace.GetComponent<Tracer>().muzzle = fpMuzzle.position;
            ApplyDamage(hit.transform);
        }
        else
        {
            GetComponentInParent<PhotonView>().RPC("CreateTracer", RpcTarget.Others, weaponId, tpMuzzle.position, cam.transform.forward * range);
            var trace = Instantiate(tracer, fpMuzzle.position, fpMuzzle.rotation);
            trace.GetComponent<Tracer>().target = cam.transform.forward * range;
            trace.GetComponent<Tracer>().muzzle = fpMuzzle.position;
        }
    }


}
