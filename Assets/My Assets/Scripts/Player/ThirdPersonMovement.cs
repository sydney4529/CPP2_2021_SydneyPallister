using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Transform groundCheck;
    public LayerMask groundMask;
    Animator anim;

    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float groundDistance = 0.4f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public ParticleSystem runPoof;

    Vector3 velocity;
    public bool isGrounded;
    public bool speedPowered;
    bool trigger;

    public bool grounded;
    private Vector3 posCur;
    private Quaternion rotCur;

    [Header("Raycast Settings")]
    public Transform thingToLookFrom;
    public float lookDistance;

    [Header("Animation Variables")]
    public bool isRunning;
    public bool isJumping;
    public bool isMoving;
    public bool isFiring;

    public AudioClip walk;
    public AudioClip speedSound;
    public AudioClip jumpSound;
    public AudioMixerGroup mixer;
    AudioSource walkSource;
    AudioSource speedSource;
    AudioSource jumpSource;

    private void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();

        if (lookDistance <= 0)
        {
            lookDistance = 10.0f;
            Debug.Log("Look distance not set on " + name + " defaulting to " + +lookDistance);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!walkSource)
        {
            walkSource = gameObject.AddComponent<AudioSource>();
            walkSource.outputAudioMixerGroup = mixer;
            walkSource.clip = walk;
            walkSource.loop = false;
            walkSource.volume = 0.2f;
        }
        if (!speedSource)
        {
            speedSource = gameObject.AddComponent<AudioSource>();
            speedSource.outputAudioMixerGroup = mixer;
            speedSource.clip = speedSound;
            speedSource.loop = false;
        }
        if (!jumpSource)
        {
            jumpSource = gameObject.AddComponent<AudioSource>();
            jumpSource.outputAudioMixerGroup = mixer;
            jumpSource.clip = jumpSound;
            jumpSource.loop = false;
        }

        if (GameManager.IsInputEnabled == true)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            //if on the ground, send the y velocity to 0 (-2 because of the checksphere)
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            //moves the player based on the direction of the camera
            if (direction.magnitude >= 0.1f && GameManager.IsInputEnabled == true)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //transform.rotation = Quaternion.Euler(0f, angle, 0f);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                isMoving = true;

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);   
            }
            else
            {
                isMoving = false;
            }

            //adds gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            //adds jumping
            if (Input.GetButtonDown("Jump") && isGrounded && GameManager.IsInputEnabled == true)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * -29.43f);
                jumpSource.Play();
            }
            if (isGrounded)
            {
                isJumping = false;
            }
            if (!isGrounded && velocity.y > 0)
            {
                isJumping = true;
            }


            if (Input.GetKey(KeyCode.LeftShift) && isMoving == true && speedPowered == false)
            {
                if (speed > 10)
                {
                    speed = 35f;
                }

            }

            if (Input.GetKeyUp(KeyCode.LeftShift) || isMoving == false && speedPowered == false)
            {
                if (speed > 10)
                {
                    speed = 20f;
                }
            }

            if (Input.GetKeyUp(KeyCode.F))
            {

            }

            if (speedPowered && !trigger)
            {
                StartCoroutine(SpeedCo());
                speedSource.Play();
            }

            if (speed > 20 && isMoving)
            {
                isRunning = true;
            }
            else
            {
                isRunning = false;
            }


            if (Input.GetButtonDown("Fire1"))
            {
                if (GameManager.IsInputEnabled == true)
                {
                    if (isGrounded && isMoving == true)
                    {
                        velocity.y = Mathf.Sqrt(2 * -2 * -29.43f);
                    }
                    anim.SetTrigger("Fire");
                }
            }

            RaycastHit hit;

            if (thingToLookFrom)
            {
                //Debug.DrawRay(thingToLookFrom.transform.position, thingToLookFrom.transform.forward * lookDistance, Color.red);

                if (Physics.Raycast(thingToLookFrom.position, thingToLookFrom.transform.forward, out hit, lookDistance))
                {
                    //Debug.Log(name + " Raycast hit: " + hit.transform.name);
                }
            }
            else
            {
                //Debug.DrawRay(transform.position, transform.forward * lookDistance, Color.green);

                if (Physics.Raycast(transform.position, transform.transform.forward, out hit, lookDistance))
                {
                    //Debug.Log(name + " Raycast hit: " + hit.transform.name);
                }
            }

            anim.SetBool("isJumping", isJumping);
            anim.SetBool("isMoving", isMoving);
            anim.SetBool("isRunning", isRunning);
            anim.SetBool("isFiring", isFiring);
            anim.SetBool("Alive", GameManager.alive);

        }
    }

    IEnumerator SpeedCo()
    {
        speed = 55f;
        trigger = true;
        runPoof.Play();
        yield return new WaitForSeconds(5);
        speed = 20;
        trigger = false;
        speedPowered = false;
        runPoof.Stop();
    }

    public void walkSound()
    {
        walkSource.Play();
    }

}
