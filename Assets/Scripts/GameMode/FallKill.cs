using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FallKill : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<NetworkPlayer>().CmdApplyDamage(100);
        }
    }
}
