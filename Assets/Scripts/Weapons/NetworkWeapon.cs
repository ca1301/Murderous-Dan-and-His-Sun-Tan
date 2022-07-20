using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkWeapon : NetworkBehaviour
{
    public NetMuzzleFlash[] netMuzzleFlashes;
    [Command(ignoreAuthority = true)]
    public void CmdShootWeapon(int id, Vector3 position, Quaternion rotation, Vector3 hitPoint)
    {
        RpcShootWeapon(id, position, rotation, hitPoint); 
    }
    [ClientRpc]
    public void RpcShootWeapon(int id, Vector3 position, Quaternion rotation, Vector3 hitPoint)
    {
        if(!isLocalPlayer)
        {
            var muz = Instantiate(netMuzzleFlashes[id].muzzleFlash, position, rotation);
            Destroy(muz, 1);
            AudioSource.PlayClipAtPoint(netMuzzleFlashes[id].fireSound, position);

            var tracer = Instantiate(netMuzzleFlashes[id].tracer, position, rotation).GetComponent<Tracer>();
            tracer.target = hitPoint;
        }
    }

}
[System.Serializable]
public struct NetMuzzleFlash
{
    public GameObject muzzleFlash;
    public AudioClip fireSound;
    public GameObject tracer;
}
