using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
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
        if (hit.tag == "Head")
        {
        }
        if (hit.tag == "Body")
        {
        }
        if (hit.tag == "Arm")
        {
        }
        if (hit.tag == "Leg")
        {
        }
        if(hit.gameObject.CompareTag("Hat"))
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
