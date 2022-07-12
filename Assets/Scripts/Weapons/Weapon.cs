using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
            hit.GetComponentInParent<PhotonView>().RPC("ApplyDamage", RpcTarget.All, headDamage);
        }
        if (hit.tag == "Body")
        {
            hit.GetComponentInParent<PhotonView>().RPC("ApplyDamage", RpcTarget.All, bodyDamage);
        }
        if (hit.tag == "Arm")
        {
            hit.GetComponentInParent<PhotonView>().RPC("ApplyDamage", RpcTarget.All, armDamage);
        }
        if (hit.tag == "Leg")
        {
            hit.GetComponentInParent<PhotonView>().RPC("ApplyDamage", RpcTarget.All, legDamage);
        }
        if(hit.gameObject.CompareTag("Hat"))
        {
            hit.GetComponent<PhotonView>().RPC("OnFallOff", RpcTarget.All);
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
