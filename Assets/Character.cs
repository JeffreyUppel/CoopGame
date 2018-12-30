﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    bool isGrounded = false;

    Vector3 groundNormal;

    Rigidbody rigidbody;
    Animator animator;

    public float jumpPower = 10f;
    public float maxMoveSpeed = 10f;
    private float currentMoveSpeed = 0;
    private Vector3 previousMove = Vector3.zero;
   
    public float acceleration = 5f;
    public float drag = 5f;
    public float directionAdjustmentSpeed = .1f;
    private float groundCheckDistance = .1f;

    private Vector3 currentVelocity;



    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();
    }

    public void Move(Vector3 move, bool jump)
    {

        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();

        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, groundNormal);
      
        // control and velocity handling is different when grounded and airborne:
        if (isGrounded)
        {
            HandleGroundedMovement(move, jump);
            UpdateAnimator(move);  //TODO right now it doesnt use the adjusted move from HandleGroundMovement I think
        }
        else
        {
            HandleAirborneMovement();
        }
    }

    void HandleGroundedMovement(Vector3 move, bool jump)
    {
        //Add the acceleration over time and clamp it between the max movespeed
        if (move != Vector3.zero)
        {
            currentMoveSpeed += acceleration;
            currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, -maxMoveSpeed, maxMoveSpeed);
        }


        //Smoothens the directional control
        if (previousMove != Vector3.zero)
        {
            move = Vector3.Slerp(previousMove, move, directionAdjustmentSpeed);
        }

        //Apply the direction and speed to the rigidbody
        Vector3 playerVelocity = move * currentMoveSpeed * Time.deltaTime;
        rigidbody.velocity = playerVelocity;
        
        //Make sure  the rigidbody stays on the ground
        move.y = 0;

        //Look at the direction
        transform.LookAt(this.transform.position + move);

        //save the direction for the directional smoothing
        previousMove = move;
    }

    void HandleAirborneMovement()
    {
        Debug.Log("Handling air movement");
    }

    void UpdateAnimator(Vector3 move)
    {
        animator.SetFloat("moveSpeed", move.magnitude); //TODO might need to use the currentMoveSpeed to do this later but works for now
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
        {
            groundNormal = hitInfo.normal;
            isGrounded = true;

        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;

        }
    }
}
