using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FallKill : MonoBehaviour
{
    //If a player happens to glitch through the map or jump off the side then this is a collision trigger and it will kill them
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<NetworkPlayer>().CmdApplyDamage(100);
        }
    }
}
