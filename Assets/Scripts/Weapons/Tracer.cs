using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    public Vector3 target;
    public Vector3 muzzle;
    public float trailSpeed;
    public AudioSource tracerSound;
    private void Start()
    {
        var speed = Vector3.Distance(muzzle, target);
        trailSpeed = trailSpeed / speed;
        StartCoroutine(CreateTracer(trailSpeed, muzzle, target));
    }


    IEnumerator CreateTracer(float speed, Vector3 Muzzle, Vector3 HitPoint)
    {
        var lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, Muzzle);
        lineRenderer.SetPosition(1, Muzzle);
        float fraction = 0;
        while (fraction < 1f)
        {
            fraction += speed;
            yield return new WaitForSeconds(0.01f);
            lineRenderer.SetPosition(1, Vector3.Lerp(lineRenderer.GetPosition(1), HitPoint, fraction));
            this.transform.position = lineRenderer.GetPosition(0);
            lineRenderer.SetPosition(0, Vector3.Lerp(lineRenderer.GetPosition(0), HitPoint, fraction / 1.1f));
        }
        Destroy(this.gameObject);
    }
}

