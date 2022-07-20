using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public class NetworkPlayer : NetworkBehaviour
{
    public Behaviour[] behavioursToEnable;
    public GameObject fp;
    public float health = 100;
    public Animator anim;

    public Transform spine;
    public Transform cam;
    public Vector3 offset;

    public SkinnedMeshRenderer[] hideInFirstPerson;
    public MeshRenderer[] hideInFirstPersonMesh;
    public Collider[] hitBoxes;

    public CameraLook camLook;

    private Quaternion netCamRotation;
    private Vector3 realPosition;
    private Quaternion realRotation;
    private int team;


   
    void Start()
    {
        
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

            if(!isServer)
            {
                GameManager.Instance.CmdGetCurrentGameState();
            }
        }
        
    }

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
    [Command(ignoreAuthority = true)]
    public void CmdApplyDamage(float amount)
    {
        RpcApplyDamage(amount);
    }
    [ClientRpc]
    public void RpcApplyDamage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            ResetPlayer();
        }
    }

    [ClientRpc]
    public void RpcSetTeam(int teamID)
    {
        this.team = teamID;
    }
    
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

    [ClientRpc]
   public  void RpcDisablePlayer(Vector3 position, Quaternion rotation)
    {
        GetComponent<CharacterController>().transform.position = position;
        GetComponent<CharacterController>().transform.rotation = rotation;
        DisablePlayer();
    }

    [ClientRpc]
    public void RpcEnablePlayer()
    {
        EnablePlayer();
    }

    public void DisablePlayer()
    {
        if (isLocalPlayer)
        {
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
