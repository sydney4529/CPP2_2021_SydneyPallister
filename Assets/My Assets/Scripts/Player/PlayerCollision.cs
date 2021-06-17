using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerCollision : MonoBehaviour
{
    ThirdPersonMovement movementScript;
    bool notTriggered;
    public Animator anim;

    public AudioClip hit;
    public AudioClip vines;
    public AudioClip death;
    public AudioMixerGroup mixerGroup;
    AudioSource hitSource;
    AudioSource vineSource;
    public AudioSource deathSource;

    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<ThirdPersonMovement>();
        anim = GetComponent<Animator>();
        notTriggered = true;
    }

    // Update is called once per frame
    void Update()
    {
        CanvasManager canvas = FindObjectOfType<CanvasManager>();

        if (!hitSource)
        {
            hitSource = gameObject.AddComponent<AudioSource>();
            hitSource.outputAudioMixerGroup = mixerGroup;
            hitSource.clip = hit;
            hitSource.loop = false;
        }
        if (!deathSource)
        {
            deathSource = gameObject.AddComponent<AudioSource>();
            deathSource.outputAudioMixerGroup = mixerGroup;
            deathSource.clip = death;
            deathSource.loop = false;
        }
        if (!vineSource)
        {
            vineSource = gameObject.AddComponent<AudioSource>();
            vineSource.outputAudioMixerGroup = mixerGroup;
            vineSource.clip = vines;
            vineSource.loop = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.alive == true && GameManager.vulnerable)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                GameManager.instance.health--;
                anim.SetTrigger("Hurt");
                hitSource.Play();
            }

            if (collision.gameObject.tag == "EnemyProjectile")
            {
                GameManager.instance.health--;
                anim.SetTrigger("Hurt");
                hitSource.Play();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(GameManager.alive == true && GameManager.vulnerable)
        {
            if (other.gameObject.tag == "Vines")
            {
                vineSource.Play();
                movementScript.speed = 10;
                movementScript.isRunning = false;
                if (notTriggered)
                {
                    StartCoroutine(Wait());
                    notTriggered = false;
                }
            }

            if (other.gameObject.tag == "Water")
            {
                GameManager.instance.health -= 3;
            }

            if (other.gameObject.tag == "Blockade")
            {
                if (notTriggered)
                {
                    StartCoroutine(Wait());
                    notTriggered = false;
                }
            }

        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Vines")
        {
            movementScript.speed = 20;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.alive == true && GameManager.vulnerable)
        {
            if (other.gameObject.tag == "Explosion")
            {
                GameManager.instance.health--;
                anim.SetTrigger("Hurt");
                hitSource.Play();
            }
        }  
    }

    IEnumerator Wait()
    {
        GameManager.instance.health--;
        anim.SetTrigger("Hurt");
        hitSource.Play();
        yield return new WaitForSeconds(2);
        notTriggered = true;
    }

    public void RespawnDragon()
    {
        if (GameManager.instance.lives > 0)
        {
            GameManager.instance.PreLoadGame();
        }
        else
        {
            GameManager.instance.GameOver();
        }
    }

    public void ApplyDamage(int damage)
    {
        GameManager.instance.health-=damage;
    }
}
