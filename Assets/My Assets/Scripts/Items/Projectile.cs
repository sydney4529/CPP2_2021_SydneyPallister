using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Projectile : MonoBehaviour
{

    public float speed;
    public float lifeTime;

    public ParticleSystem burst;
    public ParticleSystem hit;

    CapsuleCollider thisCollider;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            thisCollider = GetComponent<CapsuleCollider>();
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(ex.Message);
        }
        

        if (lifeTime <= 0)
        {
            lifeTime = 2.0f;
        }

        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 7)
        {
            Instantiate(hit, transform.position, transform.rotation);
        }
        else
        {
            Instantiate(burst, transform.position, transform.rotation);
        }
        
        Destroy(gameObject);
    }
}
