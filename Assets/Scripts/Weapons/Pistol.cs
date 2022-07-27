using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pistol : Weapon
{
    public GameObject tracer;

    


    public override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0) && bullets > 0)
        { 
            Shoot();
        }
    }

    void Shoot()
    {
        var muz = Instantiate(muzzleFlash, fpMuzzle.position, fpMuzzle.rotation);
        AudioSource.PlayClipAtPoint(fireSound, fpMuzzle.position);
        Destroy(muz, 1);
       
        Vector3 origin = cam.transform.position + cam.transform.forward * 1;
        Vector3 hitPoint = Vector3.zero;

        RaycastHit hit;
        if (Physics.Raycast(origin, cam.transform.forward, out hit, range, shootableLayers))
        {
            hitPoint = hit.point;
            ApplyDamage(hit.transform);
        }
        else
        {
            hitPoint = cam.transform.forward * range;
        }

        var trace = Instantiate(tracer, fpMuzzle.position, fpMuzzle.rotation);
        trace.GetComponent<Tracer>().target = hitPoint;
        if (isServer)
        {
            GetComponentInParent<NetworkWeapon>().RpcShootWeapon(weaponId, tpMuzzle.position, tpMuzzle.rotation, hitPoint);
        }
        else
        {
            GetComponentInParent<NetworkWeapon>().CmdShootWeapon(weaponId, tpMuzzle.position, tpMuzzle.rotation, hitPoint);
        }
    }


}
