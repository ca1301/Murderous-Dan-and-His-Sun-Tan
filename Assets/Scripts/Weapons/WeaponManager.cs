using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class WeaponManager : NetworkBehaviour
{
    public Weapon[] weapons;
    public TP_IK ik;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        ik.weapon = weapons[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (isServer)
                {
                    RpcChangeWeapon(0);
                }
                else
                {
                    CmdChangeWeapon(0);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (isServer)
                {
                    RpcChangeWeapon(1);
                }
                else
                {
                    CmdChangeWeapon(1);
                }
            }
        }
    }

    [Command]
    void CmdChangeWeapon(int id)
    {
        RpcChangeWeapon(id);
    }

    [ClientRpc]
    void RpcChangeWeapon(int id)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].thirdPersonWeapon.gameObject.SetActive(false);
            weapons[i].gameObject.SetActive(false);
        }
        weapons[id].thirdPersonWeapon.gameObject.SetActive(true);
        weapons[id].gameObject.SetActive(true);
        ik.weapon = weapons[id];
        if(isLocalPlayer)
        {
            anim.runtimeAnimatorController = weapons[id].animController;
        }
    }
}
