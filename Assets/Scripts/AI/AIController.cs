using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent nma;
    public Cover[] covers;
    public float health;
    public bool isDead;
    public int team;
    private Transform player;
    public Transform spine;
    public Vector3 offset;
    public Transform head;
    public bool canSeePlayer;


    public Transform muzzle;
    public GameObject muzzleFlash;
    public float fireRate;
    private float lastBullet;
    public int ammo;
    public int magazineSize;

    // Start is called before the first frame update
    void Start()
    {
        nma = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        covers = FindObjectsOfType<Cover>();

        this.gameObject.name = "AI: " + Random.Range(0, 100);
   

        ammo = magazineSize;

    }



    private void Update()
    {
        Vector3 playerPosition = Vector3.zero;//player.GetComponentInParent<Player>().eyes.position;
        Vector3 position = head.transform.position + head.transform.forward * 0.5f;
        anim.SetFloat("Vertical", nma.velocity.magnitude);
        if (Physics.Linecast(position, playerPosition))
        {
            canSeePlayer = false;
        }
        else
        {
            canSeePlayer = true;
        }
        
        if (canSeePlayer)
        {
            if(ammo < magazineSize / 3)
            {
                Cover newHidingSpot = covers[0];
                foreach (var item in covers)
                {
                    if (item.safe && Vector3.Distance(playerPosition, this.transform.position) < Vector3.Distance(playerPosition, newHidingSpot.transform.position) || newHidingSpot.transform.position == Vector3.zero)
                    {
                        newHidingSpot = item;
                    }
                }
                nma.SetDestination(newHidingSpot.transform.position);
                if (Vector3.Distance(this.transform.position, newHidingSpot.transform.position) < 3)
                {
                    if (newHidingSpot.crouch)
                    {
                        anim.SetBool("Crouching", true);
                    }
                    else if (!newHidingSpot.crouch)
                    {
                        anim.SetBool("Crouching", false);
                    }
                }
                else
                {
                    anim.SetBool("Crouching", false);
                }
            }
            else
            {
                if (Time.time > fireRate + lastBullet)
                {
                    lastBullet = Time.time + fireRate;
                    Shoot();
                }
            }

            if (player != null && !nma.enabled && !canSeePlayer && nma.isOnNavMesh)
            {
                Debug.Log("Setting Position");
                nma.SetDestination(player.transform.position);
            }
        }

            
        
    }

    public void LateUpdate()
    {
        if (player != null || !nma.enabled)
        {
            spine.transform.LookAt(player);
            spine.rotation = spine.rotation * Quaternion.Euler(offset);
            var lookPos = player.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f);
        }
    }

    void Shoot()
    {
        var muz = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
        Destroy(muz, 1);
    }



    public void Suicide()
    {
        health = 0;
    }


    public void ApplyDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {

        }
    }
}
  

