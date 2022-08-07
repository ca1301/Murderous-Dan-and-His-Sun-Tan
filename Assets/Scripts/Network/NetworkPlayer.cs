using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public class NetworkPlayer : NetworkBehaviour
{
    [Header("Player Setup")]
    public Behaviour[] behavioursToEnable;
    public SkinnedMeshRenderer[] hideInFirstPerson;
    public MeshRenderer[] hideInFirstPersonMesh;
    public Collider[] hitBoxes;
    public GameObject fp;
    public CameraLook camLook;


    [Header("Player Stats")]
    public float health = 100;
    private int team;

    [Header("Animation/IK")]
    public Animator anim;
    public Transform spine;
    public Transform cam;
    public Vector3 offset;
    private Quaternion netCamRotation;
    private Vector3 realPosition;
    private Quaternion realRotation;



   
    void Start()
    {
        //If player is the local player enable/disable items
        if(netIdentity.isLocalPlayer)
        {
            foreach (var item in hideInFirstPerson)
            {
                item.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            foreach (var item in hideInFirstPersonMesh)
            {
                item.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            foreach (Behaviour behaviour in behavioursToEnable)
            {
                behaviour.enabled = true;
            }
            foreach (var item in hitBoxes)
            {
                item.enabled = false;
            }
            fp.SetActive(true);

            //If is client then request the current game state
            if(!isServer)
            {
                GameManager.Instance.CmdGetCurrentGameState();
            }
        }
        
    }
    //IK spine/head player look
    public void LateUpdate()
    {
        spine.rotation = cam.rotation;
        spine.rotation = spine.rotation * Quaternion.Euler(offset);

        if(!isLocalPlayer)
        {
            cam.rotation = Quaternion.Slerp(cam.rotation, netCamRotation, 0.9f);
            spine.rotation = cam.rotation;
            spine.rotation = spine.rotation * Quaternion.Euler(offset);
        }
        
    }

    //If player shot sent the damage to the server
    [Command(ignoreAuthority = true)]
    public void CmdApplyDamage(float amount)
    {
        RpcApplyDamage(amount);
    }

    //Get the player shot message back from server
    [ClientRpc]
    public void RpcApplyDamage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            ResetPlayer();
        }
    }

    //Sent the players team to them
    [ClientRpc]
    public void RpcSetTeam(int teamID)
    {
        this.team = teamID;
    }
    
    //Send the current players stats to everyone such as position and rotation
   void Update()
   {
        if (isLocalPlayer)
        {
            if(isServer)
            {
                RpcSendData(transform.position, transform.rotation, anim.GetFloat("Vertical"), anim.GetFloat("Horizontal"), anim.GetBool("Jump"), anim.GetBool("Crouching"), cam.rotation);
            }
            else
            {
                CmdSendData(transform.position, transform.rotation, anim.GetFloat("Vertical"), anim.GetFloat("Horizontal"), anim.GetBool("Jump"), anim.GetBool("Crouching"), cam.rotation);
            }
        }
    }

    //This is used to diasable the player inbetween rounds do they cannot run around
    [ClientRpc]
   public  void RpcDisablePlayer(Vector3 position, Quaternion rotation)
    {
        GetComponent<CharacterController>().transform.position = position;
        GetComponent<CharacterController>().transform.rotation = rotation;
        DisablePlayer();
    }

    //This is used to enable the player so that they can play during the rounds
    [ClientRpc]
    public void RpcEnablePlayer()
    {
        EnablePlayer();
    }

    //
    public void DisablePlayer()
    {
        if (isLocalPlayer)
        {
            foreach (var item in behavioursToEnable)
            {
                item.enabled = false;
            }
        }
        else
        {
            //Turn off the mesh
            foreach (var item in hideInFirstPersonMesh)
            {
                item.enabled = false;
            }
            foreach (var item in hideInFirstPerson)
            {
                item.enabled = false;
            }
            foreach (var item in hitBoxes)
            {
                item.enabled = false;
            }
        }
    }

    public void ResetPlayer()
    {
        if (isLocalPlayer)
        {
            GameManager.Instance.CmdPlayerDied(netIdentity, team);
            foreach (var item in behavioursToEnable)
            {
                item.enabled = false;
            }
            //fp.SetActive(false);
        }
        else
        {
            //Turn off the mesh
            foreach (var item in hideInFirstPersonMesh)
            {
                item.enabled = false;
            }
            foreach (var item in hideInFirstPerson)
            {
                item.enabled = false;
            }
            foreach (var item in hitBoxes)
            {
                item.enabled = false;
            }
        }
    }

    public void EnablePlayer()
    {
        health = 100;
        if (isLocalPlayer)
        {
            foreach (var item in behavioursToEnable)
            {
                item.enabled = true;
            }
            //fp.SetActive(true);
        }
        else
        {
            foreach (var item in hideInFirstPersonMesh)
            {
                item.enabled = true;
            }
            foreach (var item in hitBoxes)
            {
                item.enabled = true;
            }
            foreach (var item in hideInFirstPerson)
            {
                item.enabled = true;
            }
        }
    }

    [Command]
    public void CmdSendData(Vector3 position, Quaternion rotation, float vertical, float horizontal, bool jump, bool crouch, Quaternion camRotation)
    {
        RpcSendData(position, rotation, vertical, horizontal, jump, crouch, camRotation);
    }

    [ClientRpc]
    public void RpcSendData(Vector3 position, Quaternion rotation, float vertical, float horizontal, bool jump, bool crouch, Quaternion camRotation)
    {
        if(!isLocalPlayer)
        {
            realPosition = position;
            realRotation = rotation;
            anim.SetFloat("Vertical", vertical);
            anim.SetFloat("Horizontal", horizontal);
            anim.SetBool("Jump", jump);
            anim.SetBool("Crouching", crouch);
            netCamRotation = camRotation;
        }
    }

    void FixedUpdate()
    {
        if(!isLocalPlayer)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, realPosition, 0.7f);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, realRotation, 0.7f);
        }
        
    }

}
