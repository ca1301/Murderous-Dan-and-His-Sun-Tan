using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class TP_IK : NetworkAnimator
{
    public Weapon weapon;
    private Vector3 weaponPosition;
    private Quaternion weaponRotation;
    private void Update()
    {
        if (weapon != null)
        {
            if (isLocalPlayer)
            {
                if (isServer)
                {
                    RpcSendData(weapon.thirdPersonWeapon.localPosition, weapon.thirdPersonWeapon.localRotation);
                }
                else
                {
                    CmdSendData(weapon.thirdPersonWeapon.localPosition, weapon.thirdPersonWeapon.localRotation);
                }
            }
        }
    }


    [Command]
    public void CmdSendData(Vector3 weaponPosition, Quaternion weaponRotation)
    {
        if (weapon != null)
        {
            RpcSendData(weaponPosition, weaponRotation);
        }
    }

    [ClientRpc]
    public void RpcSendData(Vector3 _weaponPosition, Quaternion _weaponRotation)
    {
        if (weapon != null)
        {
            if (!isLocalPlayer)
            {
                weaponPosition = _weaponPosition;
                weaponRotation = _weaponRotation;

                weapon.thirdPersonWeapon.localPosition = Vector3.Lerp(weapon.thirdPersonWeapon.localPosition, weaponPosition, 0.7f);
                weapon.thirdPersonWeapon.localRotation = Quaternion.Lerp(weapon.thirdPersonWeapon.localRotation, weaponRotation, 0.7f);

            }
        }
    }






    private void OnAnimatorIK()
    {
        if (weapon != null)
        {
            if (!isLocalPlayer)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);

                animator.SetIKPosition(AvatarIKGoal.RightHand, weapon.rightHandTransform.position);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, weapon.leftHandTransform.position);


                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);


                animator.SetIKRotation(AvatarIKGoal.RightHand, weapon.rightHandTransform.rotation);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, weapon.leftHandTransform.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);

                animator.SetIKPosition(AvatarIKGoal.RightHand, weapon.rightHandTransform.position);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, weapon.leftHandTransform.position);


                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);


                animator.SetIKRotation(AvatarIKGoal.RightHand, weapon.rightHandTransform.rotation);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, weapon.leftHandTransform.rotation);
            }
        }
    }
}
