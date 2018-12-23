using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter character; // A reference to the ThirdPersonCharacter on the object
        private Transform cam;                  // A reference to the main camera in the scenes transform
        private Vector3 camForward;             // The current forward direction of the camera
        private Vector3 move;
        private bool isJumping;                      // the world-relative desired move direction, calculated from the camForward and user input.

        private Vector3 attackDirection;

        [SerializeField] bool useController = true;
        
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            character = GetComponent<ThirdPersonCharacter>();
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


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float xSpeed = Input.GetAxis("Horizontal");
            float zSpeed = Input.GetAxis("Vertical");

            // calculate move direction to pass to character
            if (cam != null)
            {
                // calculate camera relative direction to move:
                camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                move = zSpeed*camForward + xSpeed*cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                move = zSpeed*Vector3.forward + xSpeed*Vector3.right;
            }

            character.Move(move, isJumping);
            isJumping = false;


        }
    }
}
