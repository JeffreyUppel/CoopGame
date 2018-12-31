using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    private bool isJumping = false;
    private Vector3 move;
    private float maxRaycastDepth = 100f;

    [SerializeField] private bool useController = false;
    private Vector3 aimDirection;
    private bool isShooting = false;

    private Character character;

    private void Start()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (!isJumping)
        {
            isJumping = Input.GetButtonDown("Jump");
        }

        if (useController)
        {
            float xAim = Input.GetAxisRaw("Controller Right Horizontal");
            float zAim = Input.GetAxisRaw("Controller Right Vertical");
            aimDirection = new Vector3(xAim, 0, zAim);
        }
        else
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 mousePos = Vector3.zero;
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxRaycastDepth);

            for (int i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].collider.tag == "Ground")
                {
                    mousePos = raycastHits[i].point;
                }
            }

            aimDirection = mousePos - this.transform.position;

            if (Input.GetButtonDown("Fire1"))
            {
                isShooting = true;
            }
        }

        character.AimBehaviour(aimDirection, isShooting);
        isShooting = false;
    }

    private void FixedUpdate()
    {
        // read inputs
        float xSpeed = Input.GetAxis("Horizontal");
        float zSpeed = Input.GetAxis("Vertical");

        // calculate move direction to pass to character
        move = new Vector3(xSpeed, 0, zSpeed);

        character.Move(move, isJumping);
        isJumping = false;
    }

}
