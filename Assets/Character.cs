using System.Collections;
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
    private float currentMoveSpeed;
    private float currentMoveSpeedX;
    private float currentMoveSpeedZ;
    public float acceleration = 5f;
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
        //move = Vector3.ProjectOnPlane(move, groundNormal);
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
        Debug.Log("Handling ground movement");

        currentMoveSpeedX = currentMoveSpeedX + move.x * acceleration;
        currentMoveSpeedX = Mathf.Clamp(currentMoveSpeedX, -maxMoveSpeed, maxMoveSpeed);

        currentMoveSpeedZ = currentMoveSpeedZ + move.z * acceleration;
        currentMoveSpeedZ = Mathf.Clamp(currentMoveSpeedZ, -maxMoveSpeed, maxMoveSpeed);

        Vector3 playerVelocity = new Vector3(currentMoveSpeedX, 0, currentMoveSpeedZ) * Time.deltaTime;

        rigidbody.velocity = playerVelocity;
        currentVelocity = playerVelocity;
        move.y = 0;
        transform.LookAt(this.transform.position + move);

        UpdateAnimator(move);
    }

    void HandleAirborneMovement()
    {
        Debug.Log("Handling air movement");
    }

    void UpdateAnimator(Vector3 move)
    {
        if (move.x != 0 && move.z != 0)
        {
            animator.SetFloat("moveSpeed", 1);
        }
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
