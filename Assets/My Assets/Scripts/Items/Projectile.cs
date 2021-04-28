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
        Destroy(gameObject);
    }
}
