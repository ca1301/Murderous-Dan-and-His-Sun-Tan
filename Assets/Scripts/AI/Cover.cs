using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public bool safe;
    public bool crouch;
    private Transform player;
    public LayerMask layer;
    private float crouchHeight = 1;
    private float standHeight = 1.5f;
    private float standHeadHeight = 1.58f;
    private float crouchHeadHeight = 1.13f;


    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().eyes != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().eyes;
        }
    }

    private void FixedUpdate()
    {
        if(player != null)
        {
            if (!crouch)
            {
                Debug.DrawLine(new Vector3(this.transform.position.x, standHeadHeight, this.transform.position.z), player.transform.position, Color.blue);
                if (Physics.Linecast(new Vector3(this.transform.position.x, standHeadHeight, this.transform.position.z), player.transform.position, layer))
                {
                    safe = true;
                }
                else
                {
                    safe = false;
                }
            }
            else
            {
                Debug.DrawLine(new Vector3(this.transform.position.x, crouchHeadHeight, this.transform.position.z), player.transform.position, Color.grey);
                if (Physics.Linecast(new Vector3(this.transform.position.x, crouchHeadHeight, this.transform.position.z), player.transform.position, layer))
                {
                    safe = true;
                }
                else
                {
                    safe = false;
                }
            }

        }
    }

    private void OnDrawGizmos()
    {
        if(safe)
        {
            Gizmos.color = Color.green;
        }   
        else
        {
            Gizmos.color = Color.red;
        }
        if(crouch)
        {
            Gizmos.DrawWireCube(new Vector3(this.transform.position.x, this.transform.position.y + (crouchHeight / 2), this.transform.position.z), new Vector3(0.4f, crouchHeight, 0.4f));
            Gizmos.DrawWireSphere(new Vector3(this.transform.position.x, this.transform.position.y + crouchHeadHeight, this.transform.position.z), 0.3f);
        }
        else
        {
            Gizmos.DrawWireCube(new Vector3(this.transform.position.x, this.transform.position.y + (standHeight / 2),this.transform.position.z), new Vector3(0.4f, standHeight, 0.4f));
            Gizmos.DrawWireSphere(new Vector3(this.transform.position.x, this.transform.position.y + standHeadHeight, this.transform.position.z), 0.3f);
        }
        
    }
}
