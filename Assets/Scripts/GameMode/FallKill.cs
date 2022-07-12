using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class FallKill : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<PhotonView>().RPC("ApplyDamage", RpcTarget.All, 100);
        }
    }
}
