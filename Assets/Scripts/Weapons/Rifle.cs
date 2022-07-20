using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    public GameObject tracer;
    public float fireRate = 0.1f;
    private float lastBullet;

    public override void Update()
    {
        base.Update();
        if (Input.GetMouseButton(0) && Time.time > fireRate + lastBullet && bullets > 0)
        {
            lastBullet = Time.time + fireRate;
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