using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class HandcontrolVRRight : MonoBehaviour {

    private Animator animator;
    VRTK_Pointer right;
    void Start()
    {
        animator = GetComponent<Animator>();
        right = GameObject.Find("RightController").GetComponent<VRTK_Pointer>();
    }

    void Update()
    {
        animator.SetBool("isGrabbing", right.controller.triggerPressed);

    }
}
