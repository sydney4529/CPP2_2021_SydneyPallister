using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerFire : MonoBehaviour
{
    public bool isFiring;
    public float projectileSpeed;

    public Transform spawnPoint;
    public Projectile projectilePrefab;

    CharacterController chara;

    // Start is called before the first frame update
    void Start()
    {
        chara = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //FireProjectile();
            //isFiring = true;
            FireProjectile();
        }
    }

    public void FireProjectile()
    {
        try
        {
            Projectile projectileInstance = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            //projectileInstance.GetComponent<Rigidbody>().velocity = transform.forward * projectileInstance.speed;
            projectileInstance.GetComponent<Rigidbody>().velocity = (transform.forward * 30) + chara.velocity; 
        }
        catch(ArgumentException ex)
        {
            Debug.Log(ex.Message);
        }
        
        //isFiring = false;
    }
}