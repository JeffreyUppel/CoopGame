using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    bool isGrounded = false;

    Vector3 groundNormal;

    Rigidbody rigidbody;
    [SerializeField] private Animator animator;

    public float jumpPower = 10f;
    public float maxMoveSpeed = 10f;
    private float currentMoveSpeed = 0;
    private Vector3 previousMove = Vector3.zero;
   
    public float acceleration = 5f;
    public float drag = 5f;
    public float directionAdjustmentSpeed = .1f;
    private float groundCheckDistance = .3f;

    private Vector3 currentMoveVelocity;
    private Vector3 currentJumpVelocity = Vector3.zero;

    [SerializeField] GameObject crosshair;
    [SerializeField] private float aimLength = 2f;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    private bool isShooting = false;
    private float shootingTimer = 0;
    private Vector3 aimDirection = Vector3.zero;
    private bool isRolling = false;



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
        currentMoveVelocity = move * currentMoveSpeed * Time.deltaTime;
        //rigidbody.velocity = playerVelocity;

        //Make sure  the rigidbody stays on the ground
        move.y = 0;

        //Look at the direction
        if (shootingTimer > 0)
        {
            Vector3 aimDir = this.transform.position + aimDirection;
            transform.LookAt(aimDir);
        }
        else
        {
            Vector3 walkDir = this.transform.position + move;
            transform.LookAt(walkDir);
        }

        //save the direction for the directional smoothing
        previousMove = move;



        // control and velocity handling is different when grounded and airborne:
        if (isGrounded)
        {
            HandleGroundedMovement(jump);
        }
        else
        {
            HandleAirborneMovement(move, jump);
        }

        rigidbody.velocity = currentMoveVelocity + currentJumpVelocity;
        UpdateAnimator(move);  //TODO right now it doesnt use the adjusted move from HandleGroundMovement I think

    }

    void HandleGroundedMovement(bool jump)
    {
        if (jump)
        {
            Debug.Log("JUmp!");
            currentJumpVelocity = Vector3.up * jumpPower;
            isGrounded = false;
        }
    }

    void HandleAirborneMovement(Vector3 move, bool jump)
    {
        currentJumpVelocity.y -= .7f;
    }

    void UpdateAnimator(Vector3 move)
    {
        animator.SetFloat("moveSpeed", move.magnitude); //TODO might need to use the currentMoveSpeed to do this later but works for now
        animator.SetBool("onGround", isGrounded);
       // animator.SetBool("isShooting", isShooting);

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

    public void AimBehaviour(Vector3 _aimDirection, bool _isShooting)
    {
        //Normalize the direction
        _aimDirection = _aimDirection.normalized;
        aimDirection = _aimDirection;
        // Vector3 dir = aimDirection - this.transform.position;
        Vector3 crosshairPos = this.transform.position + aimDirection * aimLength;
        crosshairPos.y = .1f;
        crosshair.transform.position = crosshairPos;

        if (_isShooting)
        {
            isShooting = true;
            animator.SetTrigger("isShooting");
            Shoot(aimDirection);
            shootingTimer = .2f;
            //transform.LookAt(aimDirection);
        }

        isShooting = false;
    }

    private void Shoot(Vector3 aimDirection)
    {
        GameObject bulletGOJ = Instantiate(bullet, shootPos.position, Quaternion.identity);
        Projectile b = bulletGOJ.GetComponent<Projectile>();
        b.SetTarget(aimDirection);
    }

    private void Update()
    {
        if (shootingTimer <= 0)
        {
            return;
        }

        shootingTimer -= Time.deltaTime;
    }
}
