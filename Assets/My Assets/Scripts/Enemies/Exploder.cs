using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;

public class Exploder : MonoBehaviour
{
    public Camera mainCamera;
    public Transform player;
    public float speed;

    public GameObject spawner;
    public GameObject explosion;
    public ParticleSystem deathBurst;

    float MinDist = 40f;
    float attackDist = 5f;

    Rigidbody rb;
    Animator anim;
    NavMeshAgent agent;

    public string enemyID;

    int health;

    public bool alive;

    public AudioClip enemyDeath;
    public AudioClip enemyHit;
    public AudioMixerGroup mixerGroup;
    AudioSource deathSource;
    AudioSource hitSource;

    // Start is called before the first frame update
    void Start()
    {
        if(speed <= 0)
        {
            speed = 4f;
        }

        name = "Exploder";
        health = 3;
        alive = true;

        enemyID = GetComponent<UniqueId>().uniqueId;

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (GameManager.save == true)
        {
            Debug.Log("save is true");
            LoadGameComplete();
        }
    }

    private void Update()
    {
        if (!deathSource)
        {
            deathSource = gameObject.AddComponent<AudioSource>();
            deathSource.outputAudioMixerGroup = mixerGroup;
            deathSource.clip = enemyDeath;
            deathSource.loop = false;
        }
        if (!hitSource)
        {
            hitSource = gameObject.AddComponent<AudioSource>();
            hitSource.outputAudioMixerGroup = mixerGroup;
            hitSource.clip = enemyHit;
            hitSource.loop = false;
        }

    }

    public void SaveGamePrepare()
    {
        List<LoadSaveManager.GameStateData.DataEnemy> enemies4 =
            GameManager.StateManager.gameState.enemies4;

        for (int i = 0; i < enemies4.Count; i++)
        {
            if (enemies4[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                GameManager.StateManager.gameState.enemies4.Remove(enemies4[i]);
                break;
            }
        }

        LoadSaveManager.GameStateData.DataEnemy data = new LoadSaveManager.GameStateData.DataEnemy();

        data.enemyID = GetComponent<UniqueId>().uniqueId;
        data.saveAlive = alive;
        data.health = health;

        data.posRotScale.posX = transform.position.x;
        data.posRotScale.posY = transform.position.y;
        data.posRotScale.posZ = transform.position.z;

        data.posRotScale.rotX = transform.localEulerAngles.x;
        data.posRotScale.rotY = transform.localEulerAngles.y;
        data.posRotScale.rotZ = transform.localEulerAngles.z;

        data.posRotScale.scaleX = transform.localScale.x;
        data.posRotScale.scaleY = transform.localScale.y;
        data.posRotScale.scaleZ = transform.localScale.z;

        //Add enemy to Game State
        GameManager.StateManager.gameState.enemies4.Add(data);
    }

    public void LoadGameComplete()
    {
        // Cycle through enemies and find matching ID
        List<LoadSaveManager.GameStateData.DataEnemy> enemies4 =
            GameManager.StateManager.gameState.enemies4;

        // Reference to this enemy
        LoadSaveManager.GameStateData.DataEnemy data = null;

        for (int i = 0; i < enemies4.Count; i++)
        {
            if (enemies4[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                data = enemies4[i];
                break;
            }
        }

        // If here and no enemy is found, then it was destroyed when saved. So destroy.
        if (data == null)
        {
            CanvasManager canvas = FindObjectOfType<CanvasManager>();
            canvas.explodeRef.Remove(this);
            Destroy(gameObject);
            return;
        }

        // Else load enemy data
        enemyID = data.enemyID;
        alive = data.saveAlive;
        health = data.health;

        // Set position
        transform.position = new Vector3(data.posRotScale.posX,
            data.posRotScale.posY, data.posRotScale.posZ);

        // Set rotation
        transform.localRotation = Quaternion.Euler(data.posRotScale.rotX,
            data.posRotScale.rotY, data.posRotScale.rotZ);

        // Set scale
        transform.localScale = new Vector3(data.posRotScale.scaleX,
            data.posRotScale.scaleY, data.posRotScale.scaleZ);

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(player == null)
        {
            player = FindObjectOfType<ThirdPersonMovement>().transform;
        }


        if(Vector3.Distance(transform.position, player.position) <= MinDist && alive == true)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            transform.LookAt(player);
            anim.SetBool("Moving", true);
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool("Moving", false);
        }


        if(Vector3.Distance(transform.position, player.position) <= attackDist && alive == true)
        {
            anim.SetTrigger("Attack");
        } 
        else
        {
            anim.ResetTrigger("Attack");
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (alive == true)
        {
            if (collision.gameObject.tag == "PlayerProjectile")
            {
                if (health > 1)
                {
                    hitSource.Play();
                    anim.SetTrigger("Hurt");
                    if (GameManager.instance.playerInstance.GetComponent<PlayerFire>().poweredUp == true)
                    {
                        health -= 2;
                    }
                    else
                    {
                        health--;
                    }
                }
                else
                {
                    agent.speed = 0;
                    agent.velocity = Vector3.zero;
                    rb.useGravity = false;
                    rb.detectCollisions = false;
                    alive = false;
                    anim.SetTrigger("Die");
                    CanvasManager canvas = FindObjectOfType<CanvasManager>();
                    canvas.explodeRef.Remove(this);
                    deathSource.Play();
                    if (GameManager.save == true)
                    {
                        List<LoadSaveManager.GameStateData.DataEnemy> enemies4 =
                    GameManager.StateManager.gameState.enemies4;

                        for (int i = 0; i < enemies4.Count; i++)
                        {
                            if (enemies4[i].enemyID == enemyID)
                            {
                                // Found enemy. Now break break from loop
                                GameManager.StateManager.gameState.enemies4.Remove(enemies4[i]);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void Die()
    {
        Vector3 spawn = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
        Instantiate(spawner, spawn, Quaternion.identity);
        Instantiate(deathBurst, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void Explode()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }


}
