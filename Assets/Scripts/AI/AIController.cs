using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class AIController : MonoBehaviourPunCallbacks
{
    private Animator anim;
    private NavMeshAgent nma;
    public Cover[] covers;
    private PhotonView pv;
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
        pv = GetComponent<PhotonView>();

        this.gameObject.name = "AI: " + Random.Range(0, 100);
        object[] content = new object[] { this.gameObject.name};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_JOINED, content, raiseEventOptions, sendOptions);

        ammo = magazineSize;

        var player = FindObjectOfType<Player>();
        if (player != null)
        {
            this.player = player.transform;
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void Update()
    {
        Vector3 playerPosition = player.GetComponentInParent<Player>().eyes.position;
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



    [PunRPC]
    public void Suicide()
    {
        health = 0;
        Die();
        isDead = true;
    }


    [PunRPC]
    public void ApplyDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !isDead)
        {
            Die();
            isDead = true;
        }
    }

    public void Die()
    {
        if (pv.IsMine)
        {
            object[] content = new object[] { gameObject.name };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_DIED, content, raiseEventOptions, sendOptions);

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var item in players)
            {
                if (item.gameObject != this.gameObject)
                {
                    item.GetComponent<PhotonView>().RPC("AddHat", RpcTarget.All);
                }
            }
        }
    }




    private void OnEvent(EventData obj)
    {
        if (obj.Code == OneVsOne_Settings.GAME_START)
        {
            object[] info = (object[])obj.CustomData;
            health = 100;
        }

        if (obj.Code == OneVsOne_Settings.PLAYER_DIED)
        {
            object[] info = (object[])obj.CustomData;
            health = 100;
            nma.enabled = false;
        }

        if (obj.Code == OneVsOne_Settings.PLAYER_JOINED)
        {
            object[] info = (object[])obj.CustomData;
            if ((string)info[0] == this.gameObject.name)
            {
                object[] content = new object[] { this.gameObject.name };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_JOIN_TEAM, content, raiseEventOptions, sendOptions);
            }
        }

        if (obj.Code == OneVsOne_Settings.PLAYER_SET_UI)
        {
            object[] info = (object[])obj.CustomData;
            if ((string)info[0] == this.gameObject.name)
            {
                if ((int)info[1] == 0)
                {
                    //health_Text.color = Color.red;
                }
                else
                {
                    //health_Text.color = Color.blue;
                }
            }
        }
        if (obj.Code == OneVsOne_Settings.PLAYER_ROUND_RESET)
        {
            object[] info = (object[])obj.CustomData;
            health = 100;
            nma.enabled = false;
            if ((string)info[0] == this.gameObject.name)
            {
                isDead = false;
                this.transform.position = (Vector3)info[1];
                this.transform.rotation = (Quaternion)info[2];
                /*
                foreach (SkinnedMeshRenderer item in netPlayer.hideInFirstPerson)
                {
                    item.enabled = true;
                }
                if (pv.IsMine)
                {
                    GetComponent<PlayerController>().enabled = false;
                    GetComponent<CharacterController>().enabled = false;
                    camLook.enabled = false;
                    foreach (var item in behavioursToDisableOnRoundEnd)
                    {
                        item.enabled = false;
                    }
                }
                */
            }

        }

        if (obj.Code == OneVsOne_Settings.ROUND_READY)
        {
            object[] info = (object[])obj.CustomData;
            if ((string)info[0] == this.gameObject.name)
            {
                isDead = false;
                /*
                if (pv.IsMine)
                {
                    GetComponent<PlayerController>().enabled = true;
                    GetComponent<CharacterController>().enabled = true;
                    camLook.enabled = true;
                    foreach (var item in behavioursToDisableOnRoundEnd)
                    {
                        item.enabled = true;
                    }
              
                    pv.RPC("RPC_SHOWBODY", RpcTarget.Others);
                }
                 */
                nma.enabled = true;
                nma.SetDestination(player.transform.position);
                
            }

        }

    }



}
