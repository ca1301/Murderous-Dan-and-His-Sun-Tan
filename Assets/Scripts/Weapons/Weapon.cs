using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
    public string weaponName;
    public float damage;
    public float range;
    public Animator armsAnim;
    public Animator weaponAnim;

    public float headDamage = 100;
    public float bodyDamage = 50;
    public float legDamage = 20;
    public float armDamage = 10;

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
            if (hit.gameObject.CompareTag("Hat"))
            {

            }
       
    }



    public IEnumerator EndFire(float fireRate)
    {
        yield return new WaitForSeconds(fireRate);
        armsAnim.SetBool("Fire", false);
    }
    public IEnumerator EndReload(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);
    }
}
