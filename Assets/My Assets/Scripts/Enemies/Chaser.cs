using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;

public class Chaser : MonoBehaviour
{
    public Camera mainCamera;
    public Transform player;
    public float speed;

    public GameObject spawner;
    public ParticleSystem deathBurst;

    float MinDist = 40f;
    float attackDist = 3f;

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

        name = "Chaser";
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
        List<LoadSaveManager.GameStateData.DataEnemy> enemies3 =
            GameManager.StateManager.gameState.enemies3;

        for (int i = 0; i < enemies3.Count; i++)
        {
            if (enemies3[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                GameManager.StateManager.gameState.enemies3.Remove(enemies3[i]);
                break;
            }
        }

        //GameManager.StateManager.gameState.enemies.Remove(this);
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
        GameManager.StateManager.gameState.enemies3.Add(data);
    }

    public void LoadGameComplete()
    {


        // Cycle through enemies and find matching ID
        List<LoadSaveManager.GameStateData.DataEnemy> enemies3 =
            GameManager.StateManager.gameState.enemies3;

        // Reference to this enemy
        LoadSaveManager.GameStateData.DataEnemy data = null;

        for (int i = 0; i < enemies3.Count; i++)
        {
            if (enemies3[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                data = enemies3[i];
                break;
            }
        }

        // If here and no enemy is found, then it was destroyed when saved. So destroy.
        if (data == null)
        {
            Debug.Log("was null");
            //CanvasManager canvas = FindObjectOfType<CanvasManager>();
            CanvasManager canvas = FindObjectOfType<CanvasManager>();
            canvas.chaseRef.Remove(this);
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

        if(alive == true)
        {
            if (Vector3.Distance(transform.position, player.position) <= MinDist)
            {
                if(Vector3.Distance(transform.position, player.position) > attackDist)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.transform.position);
                    transform.LookAt(player);
                    anim.SetBool("Moving", true);
                }
            }
            else
            {
                agent.isStopped = true;
                anim.SetBool("Moving", false);
            }


            if (Vector3.Distance(transform.position, player.position) <= attackDist)
            {
                anim.SetTrigger("Attack");
            }
            else
            {
                //anim.ResetTrigger("Attack");
            }
        }
        

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile")
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
                if (alive == true)
                {
                    agent.speed = 0;
                    agent.velocity = Vector3.zero;
                    rb.useGravity = false;
                    rb.detectCollisions = false;
                    alive = false;
                    anim.SetTrigger("Die");
                    CanvasManager canvas = FindObjectOfType<CanvasManager>();
                    canvas.chaseRef.Remove(this);
                    if (GameManager.save == true)
                    {


                        List<LoadSaveManager.GameStateData.DataEnemy> enemies3 =
                    GameManager.StateManager.gameState.enemies3;

                        for (int i = 0; i < enemies3.Count; i++)
                        {
                            if (enemies3[i].enemyID == enemyID)
                            {
                                // Found enemy. Now break break from loop
                                GameManager.StateManager.gameState.enemies3.Remove(enemies3[i]);
                                break;
                            }
                        }
                    }
                    deathSource.Play();
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


}
