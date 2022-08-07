using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    [HideInInspector]
    public bool safe;

    public bool crouch;
    public LayerMask layer;
    private Transform player;

    private float crouchHeight = 1;
    private float standHeight = 1.5f;
    private float standHeadHeight = 1.58f;
    private float crouchHeadHeight = 1.13f;



    private void FixedUpdate()
    {
        
        if (player == null)
        {
            return;
        }
        //Linecast from crouch height to accurately represent if this cover is safe or not
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
            //Linecast from standing height to accurately represent if this cover is safe or not
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

    private void OnDrawGizmos()
    {
        //Colour code within scene view to quickly determine any bugs
        if (safe)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        //Display cover height and position depending if its a crouch or standing cover object
        if (crouch)
        {
            Gizmos.DrawWireCube(new Vector3(this.transform.position.x, this.transform.position.y + (crouchHeight / 2), this.transform.position.z), new Vector3(0.4f, crouchHeight, 0.4f));
            Gizmos.DrawWireSphere(new Vector3(this.transform.position.x, this.transform.position.y + crouchHeadHeight, this.transform.position.z), 0.3f);
        }
        else
        {
            Gizmos.DrawWireCube(new Vector3(this.transform.position.x, this.transform.position.y + (standHeight / 2), this.transform.position.z), new Vector3(0.4f, standHeight, 0.4f));
            Gizmos.DrawWireSphere(new Vector3(this.transform.position.x, this.transform.position.y + standHeadHeight, this.transform.position.z), 0.3f);
        }

    }
}
