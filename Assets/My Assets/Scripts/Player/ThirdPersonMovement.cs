using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Transform groundCheck;
    public LayerMask groundMask;

    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float groundDistance = 0.4f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 velocity;
    bool isGrounded;

    public bool grounded;
    private Vector3 posCur;
    private Quaternion rotCur;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks to see if the player is groundded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //if on the ground, send the y velocity to 0 (-2 because of the checksphere)
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //moves the player based on the direction of the camera
        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        //adds jumping
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            //velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * -29.43f);
        }

        //adds gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 35f;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 20f;
        }

        ////declare a new Ray. It will start at this object's position and it's direction will be straight down from the object (in local space, that is)
        //Ray ray = new Ray(transform.position, -transform.up);
        ////decalre a RaycastHit. This is neccessary so it can get "filled" with information when casting the ray below.
        //RaycastHit hit;
        ////cast the ray. Note the "out hit" which makes the Raycast "fill" the hit variable with information. The maximum distance the ray will go is 1.5
        //if (Physics.Raycast(ray, out hit, 1.5f) == true)
        //{
        //    //draw a Debug Line so we can see the ray in the scene view. Good to check if it actually does what we want. Make sure that it uses the same values as the actual Raycast. In this case, it starts at the same position, but only goes up to the point that we hit.
        //    Debug.DrawLine(transform.position, hit.point, Color.green);
        //    //store the roation and position as they would be aligned on the surface
        //    rotCur = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        //    posCur = new Vector3(transform.position.x, hit.point.y, transform.position.z);

        //    grounded = true;

        //}
        ////if you raycast didn't hit anything, we are in the air and not grounded.
        //else
        //{
        //    grounded = false;
        //}


        //if (grounded == true)
        //{
        //    //smoothly rotate and move the objects until it's aligned to the surface. The "5" multiplier controls how fast the changes occur and could be made into a seperate exposed variable
        //    transform.position = Vector3.Lerp(transform.position, posCur, Time.deltaTime * 5);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, rotCur, Time.deltaTime * 5);
        //}
        //else
        //{
        //    //if we are not grounded, make the object go straight down in world space (simulating gravity). the "1f" multiplier controls how fast we descend.
        //    transform.position = Vector3.Lerp(transform.position, transform.position - Vector3.up * 1f, Time.deltaTime * 5);
        //    //from memory, I'm not sure why I aded this... Looks like a fail safe to me. When the object is turned too much towards teh front or back, almost instantly (*1000) make it rotate to a better orientation for aligning.
        //    if (transform.eulerAngles.x > 15)
        //    {
        //        //turnVector.x -= Time.deltaTime * 1000;
        //    }
        //    else if (transform.eulerAngles.x < 15)
        //    {
        //        //turnVector.x += Time.deltaTime * 1000;
        //    }
        //    //if we are not grounded, make the vehicle's rotation "even out". Not completey reaslistic, but easy to work with.
        //    rotCur.eulerAngles = Vector3.zero;
        //    transform.rotation = Quaternion.Lerp(transform.rotation, rotCur, Time.deltaTime);

        //}
    }


}
