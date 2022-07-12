using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : MonoBehaviourPunCallbacks
{
    public float health = 100;
    public int team;
    public PhotonView pv;
    public NetworkPlayer setup;
    public GameObject ragdoll;
    public CameraLook camLook;
    public NetworkPlayer netPlayer;
    public Transform eyes;
    public Transform cam;
    public bool isDead = false;
    float eyeHeight;
    public TMP_Text health_Text;
    public Slider healthSlider;
    public List<Wearable> hats = new List<Wearable>();
    public Transform hatSpawnPos;
    private static GameObject LocalPlayerInstance;

    public Behaviour[] behavioursToDisableOnRoundEnd;

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
        health_Text.text = health.ToString();
        healthSlider.value = health;
    }
    private void LateUpdate()
    {
        eyeHeight = Vector3.Distance(this.transform.position, eyes.position);
        cam.position = new Vector3(cam.position.x, this.transform.position.y + eyeHeight, cam.transform.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        LocalPlayerInstance = this.gameObject;
        pv.RPC("UpdateName", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.NickName);
        object[] content = new object[] { this.pv.Owner.NickName };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_JOINED, content, raiseEventOptions, sendOptions);
        health_Text.text = health.ToString();
    }


    [PunRPC]
    public void AddHat()
    {
            var wearables = OneVsOne.Instance.hats;
            int hatIndex = Random.Range(0, wearables.Count);
            if (hats.Count == 0)
            {
                var hat = Instantiate(wearables[hatIndex].gameObject, hatSpawnPos.position, hatSpawnPos.rotation);
                hat.transform.parent = hatSpawnPos;
                if (pv.IsMine)
                {
                    hat.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
                hats.Add(hat.GetComponent<Wearable>());
            return;
            }
            else
            {
                var hat = Instantiate(wearables[hatIndex].gameObject, hats[hats.Count - 1].nextHatSpawnPoint.position, hats[hats.Count - 1].nextHatSpawnPoint.rotation);
                hat.transform.parent = hats[hats.Count - 1].nextHatSpawnPoint;
                if (pv.IsMine)
                {
                    hat.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
                hats.Add(hat.GetComponent<Wearable>());
            }
    }

    [PunRPC]
    public void UpdateName(string name)
    {
        this.transform.name = name;
    }

    private void OnApplicationQuit()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_LEFT, content, raiseEventOptions, sendOptions);
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
     
        if(health <= 0 && !isDead)
        {
            Die();
            isDead = true;
        }
    }
    
    public void Die()
    {
        if(pv.IsMine)
        {
            foreach (var item in behavioursToDisableOnRoundEnd)
            {
                item.enabled = false;
            }
            object[] content = new object[] { PhotonNetwork.LocalPlayer.NickName };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_DIED, content, raiseEventOptions, sendOptions);

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var item in players)
            {
                if(item.gameObject != this.gameObject)
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
        }

        if (obj.Code == OneVsOne_Settings.PLAYER_JOINED)
        {
            object[] info = (object[])obj.CustomData;
            if ((string)info[0] == this.photonView.Owner.NickName)
            {
                object[] content = new object[] { PhotonNetwork.LocalPlayer.NickName };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_JOIN_TEAM, content, raiseEventOptions, sendOptions);
            }
        }

        if (obj.Code == OneVsOne_Settings.PLAYER_SET_UI)
        {
            object[] info = (object[])obj.CustomData;
            if ((string)info[0] == this.photonView.Owner.NickName)
            {
                if ((int)info[1] == 0)
                {
                    health_Text.color = Color.red;
                }
                else
                {
                    health_Text.color = Color.blue;
                }
            }
        }
        if (obj.Code == OneVsOne_Settings.PLAYER_ROUND_RESET)
        {
            object[] info = (object[])obj.CustomData;
            health = 100;
            if ((string)info[0] == this.pv.Owner.NickName)
            {
                isDead = false;
                this.transform.position = (Vector3)info[1];
                this.transform.rotation = (Quaternion)info[2];
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
            }

        }

        if (obj.Code == OneVsOne_Settings.ROUND_READY)
        {
            object[] info = (object[])obj.CustomData;
            if ((string)info[0] == this.pv.Owner.NickName)
            {
                isDead = false;
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
            }

        }

    }

    [PunRPC]
    public void RPC_DIE()
    {
        Debug.Log("RPC_DIE");
    }

    [PunRPC]
    public void RPC_SHOWBODY()
    {
        Debug.Log("Show Body");
    }
}
