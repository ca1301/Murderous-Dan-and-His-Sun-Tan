using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FallKill : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");
        if(collision.transform.CompareTag("Player"))
        {
        }
    }
}
