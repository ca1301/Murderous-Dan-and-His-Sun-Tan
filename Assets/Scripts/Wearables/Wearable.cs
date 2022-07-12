using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wearable : MonoBehaviour, IWearable
{
    public Transform nextHatSpawnPoint;
    private Rigidbody rb;
    
    public void OnFallOff()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        this.transform.parent = null;
        GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    public void OnPickup()
    {
    }
}
