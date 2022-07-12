using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public class NetworkPlayer : NetworkBehaviour
{
    public Behaviour[] behavioursToEnable;
    public GameObject fp;

    private Vector3 realPosition;
    private Quaternion realRotation;


    public Animator anim;

    public Transform spine;
    public Transform cam;
    public Vector3 offset;

    public SkinnedMeshRenderer[] hideInFirstPerson;
    public MeshRenderer[] hideInFirstPersonMesh;
    public Collider[] hitBoxes;

    public CameraLook camLook;

    Quaternion netCamRotation;

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
                OneVsOne.Instance.CmdGetCurrentGameState();
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
