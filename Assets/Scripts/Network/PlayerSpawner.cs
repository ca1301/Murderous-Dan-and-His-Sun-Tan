using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public GameObject ai;

    public bool playWithAI;
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonNetwork.Instantiate(player.name, new Vector3(0, 6, 0), Quaternion.identity);
        }
        else
        {
            PhotonNetwork.LoadLevel(0);
        }
        if(PhotonNetwork.IsConnected && PhotonNetwork.InRoom && playWithAI)
        {
            PhotonNetwork.Instantiate(ai.name, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

}
