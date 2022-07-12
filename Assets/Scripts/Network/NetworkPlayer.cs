using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkPlayer : MonoBehaviourPunCallbacks, IPunObservable
{

    public Behaviour[] behavioursToEnable;
    public GameObject fp;

    private PhotonView pv;

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
        pv = GetComponent<PhotonView>();
        if(pv.IsMine)
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
        }
        
    }

    public void LateUpdate()
    {
        spine.rotation = cam.rotation;
        spine.rotation = spine.rotation * Quaternion.Euler(offset);
        if(!pv.IsMine)
        {
            cam.rotation = Quaternion.Slerp(cam.rotation, netCamRotation, 0.1f);

            spine.rotation = cam.rotation;
            spine.rotation = spine.rotation * Quaternion.Euler(offset);
        }
    }


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(this.transform.position);
            stream.SendNext(this.transform.rotation);
            stream.SendNext(anim.GetFloat("Vertical"));
            stream.SendNext(anim.GetFloat("Horizontal"));
            stream.SendNext(anim.GetBool("Jump"));
            stream.SendNext(anim.GetBool("Crouching"));
            stream.SendNext(cam.rotation);
        }
        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            anim.SetFloat("Vertical", (float)stream.ReceiveNext());
            anim.SetFloat("Horizontal", (float)stream.ReceiveNext());
            anim.SetBool("Jump", (bool)stream.ReceiveNext());
            anim.SetBool("Crouching", (bool)stream.ReceiveNext());
            netCamRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void FixedUpdate()
    {
        if(!pv.IsMine)
        {
            this.transform.position = Vector3.Slerp(this.transform.position, realPosition, 0.1f);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, realRotation, 0.1f);
        }
        
    }

}
