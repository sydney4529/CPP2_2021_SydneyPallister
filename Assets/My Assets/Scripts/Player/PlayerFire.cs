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
    public Projectile poweredUpPrefab;

    public bool poweredUp;
    bool trigger;

    CharacterController chara;

    // Start is called before the first frame update
    void Start()
    {
        chara = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.IsInputEnabled == true)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //FireProjectile();
                //isFiring = true;
                FireProjectile();
            }
        }

        if(poweredUp && !trigger)
        {
            trigger = true;
            StartCoroutine(WaitForFire(5));
        }
    }

    public void FireProjectile()
    {
        try
        {
            if(!poweredUp)
            {
                Projectile projectileInstance = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
                projectileInstance.GetComponent<Rigidbody>().velocity = transform.forward * projectileInstance.speed;
                //projectileInstance.transform.rotation.x = projectileInstance.transform.rotation.x * transform.rotation.x;
                projectileInstance.transform.rotation = transform.rotation;
            }
            else
            {
                Projectile projectileInstance = Instantiate(poweredUpPrefab, spawnPoint.position, Quaternion.identity);
                projectileInstance.GetComponent<Rigidbody>().velocity = transform.forward * projectileInstance.speed;
                projectileInstance.transform.rotation = transform.rotation;
            }
            
        }
        catch(ArgumentException ex)
        {
            Debug.Log(ex.Message);
        }
        
        //isFiring = false;
    }

    IEnumerator WaitForFire(float t)
    {
        yield return new WaitForSeconds(t);
        poweredUp = false;
        trigger = false;
    }
}
