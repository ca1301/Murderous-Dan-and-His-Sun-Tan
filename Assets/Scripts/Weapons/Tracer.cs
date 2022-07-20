using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    public Vector3 target;
    public float trailSpeed;
    public AudioClip tracerSound;

    private void Start()
    {
        AudioSource.PlayClipAtPoint(tracerSound, transform.position);
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(this.transform.position, target, trailSpeed * Time.deltaTime);
        
        if(transform.position == target)
        {
            Destroy(this.gameObject);
        }
        
    }

}

