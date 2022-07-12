using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkWeapon : MonoBehaviour
{
    public NetMuzzleFlash[] netMuzzleFlashes;

    [PunRPC]
    public void CreateMuzzleFlash(int id, Vector3 position, Quaternion rotation)
    {
        var muz = Instantiate(netMuzzleFlashes[id].muzzleFlash, position, rotation);
        Destroy(muz, 1);
        AudioSource.PlayClipAtPoint(netMuzzleFlashes[id].fireSound, position);
    }
    [PunRPC]
    public void CreateTracer(int id, Vector3 position, Vector3 hitPoint)
    {
        var muz = Instantiate(netMuzzleFlashes[id].tracer, position, Quaternion.identity);
        Destroy(muz, 1);
        var trace = Instantiate(netMuzzleFlashes[id].tracer, position, Quaternion.identity);
        trace.GetComponent<Tracer>().target = hitPoint;
        trace.GetComponent<Tracer>().muzzle = position;

    }
}
[System.Serializable]
public struct NetMuzzleFlash
{
    public GameObject muzzleFlash;
    public AudioClip fireSound;
    public GameObject tracer;
}
