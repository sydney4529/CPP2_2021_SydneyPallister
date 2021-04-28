using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]

public class Character : MonoBehaviour
{
    CharacterController controller;

    public float speed;
    public float jumpSpeed;
    public float rotationSpeed;
    public float gravity;

    Vector3 moveDirection;
    enum ControllerType { SimpleMove, Move };
    [SerializeField] ControllerType type;

    [Header("Weapon Settings")]
    public float projectileForce;
    public Transform projectileSpawnPoint;
    public Rigidbody projectilePrefab;

    [Header("Raycast Settings")]
    public Transform thingToLookFrom;
    public float lookDistance;

    // Start is called before the first frame update
    void Start()
    {
        try
        {

            controller = GetComponent<CharacterController>();
            controller.minMoveDistance = 0.0f;

         if(speed <= 0)
            {
                speed = 6.0f;
                Debug.Log("Default speed: " + speed + " on " + name);
            }

            if (jumpSpeed <= 0)
            {
                jumpSpeed = 6.0f;
                Debug.Log("Default speed: " + jumpSpeed + " on " + name);
            }

            if (rotationSpeed <= 0)
            {
                speed = 10.0f;
                Debug.Log("Default speed: " + rotationSpeed + " on " + name);
            }

            if (gravity <= 0)
            {
                speed = 9.81f;
                Debug.Log("Default speed: " + gravity + " on " + name);
            }
            if (projectileForce <= 0)
            {
                speed = 10.0f;
                Debug.Log("Default speed: " + projectileForce + " on " + name);
            }
            if (!projectileSpawnPoint)
            {
                Debug.Log("Missing projectile spawn point");
            }
            if (!projectilePrefab)
            {
                Debug.Log("Missing projectile prefab");
            }

            moveDirection = Vector3.zero;

        }
        catch(NullReferenceException e)
        {
            Debug.LogWarning(e.Message);
        }
        catch (UnassignedReferenceException e)
        {
            Debug.LogWarning(e.Message);
        }
        finally
        {
            Debug.Log("always gets called");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(type)
        {
            case ControllerType.SimpleMove:
                controller.SimpleMove(transform.forward * Input.GetAxis("Vertical") * speed);
                break;

            case ControllerType.Move:
                if(controller.isGrounded)
                {
                    moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                    moveDirection *= speed;

                    moveDirection = transform.TransformDirection(moveDirection);

                    if(Input.GetButtonDown("Jump"))
                    {
                        moveDirection.y = jumpSpeed;
                    }
                }

                moveDirection.y -= gravity * Time.deltaTime;

                controller.Move(moveDirection * Time.deltaTime);

                break;
        }

        if(Input.GetButtonDown("Fire1"))
        {
            fire();
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
    }

    void fire()
    {
        Debug.Log("Fired");
        Rigidbody temp = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        //shoot projectile
        temp.AddForce(projectileSpawnPoint.forward * projectileForce, ForceMode.Impulse);
        Destroy(temp.gameObject, 2.0f)
;    }

    [ContextMenu("Reset Stats")]
    void ResetStats()
    {
        speed = 0.0f;
    }

    //both objects need colliders, one or both need a Rigid body. 
    //With character controller, add IsKinematic
    //called once on first collision
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(name + " : Oncollision Enter " + collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(name + " : OnTrigger Enter " + other.gameObject.name);
    }

    //called as long as the collision is happening
    //game object needs a character controller component
    //one of the game objects needs a collider
    //behaves like oncollision stay
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

    }
}
