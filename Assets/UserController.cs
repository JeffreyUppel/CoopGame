using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    private bool isJumping = false;
    private Vector3 move; 

    [SerializeField] private bool useController = false;

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
            Vector3 aimDirection = new Vector3(xAim, 0, zAim);
        }
        else
        {
            //Using mouse
            Vector3 aimDirection = Input.mousePosition - this.transform.position;
        }


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
