using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimExample : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Transform lookObj;
    [SerializeField] private Transform grabObj;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, grabObj.position);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, grabObj.rotation);

        anim.SetLookAtWeight(1.0f);
        anim.SetLookAtPosition(lookObj.position);
    }

}
