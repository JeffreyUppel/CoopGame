using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    bool isGrounded = false;

    Vector3 groundNormal;

    Rigidbody rigidbody;

    public float jumpPower = 10f;
    public float moveSpeed = 10f;
    private float groundCheckDistance = .1f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    public void Move(Vector3 move, bool jump)
    {

        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();

        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, groundNormal);
        //turnAmount = Mathf.Atan2(move.x, move.z);
        //forwardAmount = move.z;

        //ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (isGrounded)
        {
            HandleGroundedMovement(move, jump);
        }
        else
        {
            HandleAirborneMovement();
        }
        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }

    void HandleGroundedMovement(Vector3 move, bool jump)
    {

        Vector3 playerMove = move * moveSpeed * Time.deltaTime;
        rigidbody.velocity = new Vector3(playerMove.x * moveSpeed, 0, playerMove.z * moveSpeed);
        //Vector3 dir = move - this.transform.position;
        move.y = 0;
        transform.LookAt(move);
    }

    void HandleAirborneMovement()
    {

    }

    void UpdateAnimator(Vector3 move)
    {

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
