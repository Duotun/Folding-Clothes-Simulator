using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class HandcontrolVRLeft : MonoBehaviour {
    private Animator animator;
    VRTK_Pointer left;
    void Start()
    {
        animator = GetComponent<Animator>();
        left = GameObject.Find("LeftController").GetComponent<VRTK_Pointer>();
    }

    void LateUpdate()  //confirm? whay late
    {
        if(animator!=null&&left!=null)
        animator.SetBool("isGrabbing", left.controller.triggerPressed);

    }
}
