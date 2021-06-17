using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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

    public AudioClip fire;
    public AudioClip firePower;
    public AudioMixerGroup mixerGroup;
    AudioSource fireSource;
    AudioSource firePowerSource;

    // Start is called before the first frame update
    void Start()
    {
        chara = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!fireSource)
        {
            fireSource = gameObject.AddComponent<AudioSource>();
            fireSource.outputAudioMixerGroup = mixerGroup;
            fireSource.clip = fire;
            fireSource.loop = false;
        }
        if (!firePowerSource)
        {
            firePowerSource = gameObject.AddComponent<AudioSource>();
            firePowerSource.outputAudioMixerGroup = mixerGroup;
            firePowerSource.clip = firePower;
            firePowerSource.loop = false;
        }

        if (GameManager.IsInputEnabled == true)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                FireProjectile();
                fireSource.Play();
            }
        }

        if(poweredUp && !trigger)
        {
            firePowerSource.Play();
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
        
    }

    IEnumerator WaitForFire(float t)
    {
        yield return new WaitForSeconds(t);
        poweredUp = false;
        trigger = false;
    }
}
