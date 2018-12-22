using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour 
{
	[SerializeField] private Rigidbody rigidbody;
	[SerializeField] private float moveSpeed = 500;
	[SerializeField] private float jumpPower = 500;

	private Vector3 move = Vector3.zero;
	private Vector3 jump = Vector3.zero;

			public void Move(Vector3 move, bool crouch, bool jump)
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborne:
			if (m_IsGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement();
			}

			ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}

	void FixedUpdate () 
	{
		float axisX = Input.GetAxisRaw("Horizontal");
		float axisZ = Input.GetAxisRaw("Vertical");

		if (axisX != 0 || axisZ != 0)
		{
			move = new Vector3(axisX, 0, axisZ) * moveSpeed * Time.deltaTime;
			rigidbody.velocity = move * Time.deltaTime;	

			Vector3 dir = move - this.transform.position;
			Quaternion rot = Quaternion.LookRotation(dir);
			rigidbody.rotation = rot;
		}
		else
		{
			move = Vector3.zero;
		}

		if (Input.GetButtonDown("Jump"))
		{
			Debug.Log("Jump!");
			jump = Vector3.up * jumpPower * Time.deltaTime;
		}
		else
		{
			jump = Vector3.zero;
		}

		rigidbody.velocity = move + jump;

	}
}
