using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public Camera mainCamera;
    public Transform player;
    public float speed;
    public Material invisible;
    public Material visible;
    public GameObject spawner;
    public ParticleSystem deathBurst;

    float MinDist = 40f;
    float attackDist = 5f;

    Vector3 MinDistVect;

    Rigidbody rb;
    Animator anim;
    NavMeshAgent agent;

    public string enemyID;

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

        name = "Ghost";

        alive = true;
        MinDistVect = new Vector3(40, 0, 40);

        enemyID = GetComponent<UniqueId>().uniqueId;

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        deathSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        if(GameManager.save == true)
        {
            LoadGameComplete();
        }
    }

    public void SaveGamePrepare()
    {
        List<LoadSaveManager.GameStateData.DataEnemy> enemies =
            GameManager.StateManager.gameState.enemies;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                GameManager.StateManager.gameState.enemies.Remove(enemies[i]);
                break;
            }
        }

        LoadSaveManager.GameStateData.DataEnemy data = new LoadSaveManager.GameStateData.DataEnemy();

        data.enemyID = GetComponent<UniqueId>().uniqueId;
        data.saveAlive = alive;

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
        GameManager.StateManager.gameState.enemies.Add(data);
    }

    public void LoadGameComplete()
    {
        // Cycle through enemies and find matching ID
        List<LoadSaveManager.GameStateData.DataEnemy> enemies =
            GameManager.StateManager.gameState.enemies;

        // Reference to this enemy
        LoadSaveManager.GameStateData.DataEnemy data = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                data = enemies[i];
                break;
            }
        }

        // If here and no enemy is found, then it was destroyed when saved. So destroy.
        if (data == null)
        {
            CanvasManager canvas = FindObjectOfType<CanvasManager>();
            canvas.gRef.Remove(this);
            Destroy(gameObject);
            return;
        }

        // Else load enemy data
        enemyID = data.enemyID;
        alive = data.saveAlive;

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

    // Update is called once per frame
    void FixedUpdate()
    {

        float dot = Vector3.Dot(player.forward, (transform.position - player.position).normalized);

        if (dot > 0.7f) 
        {
            if(Vector3.Distance(transform.position, player.position) <= MinDistVect.x && alive == true)
            {
                if(player.position.y <= transform.position.y + 2)
                {
                    agent.speed = 0;
                    agent.velocity = Vector3.zero;
                    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, player.transform.rotation, 2.5f);
                    if (alive == true)
                    {
                        GetComponentInChildren<SkinnedMeshRenderer>().material = invisible;
                    }
                    anim.SetBool("Moving", false);
                }
            }
        }
        else
        {
            if(Vector3.Distance(transform.position, player.position) <= MinDist && alive == true)
            {
                if (Vector3.Distance(transform.position, player.position) > attackDist)
                {
                    agent.speed = 12;
                    agent.SetDestination(player.transform.position);

                    transform.LookAt(player);
                    if (alive == true)
                        GetComponentInChildren<SkinnedMeshRenderer>().material = visible;
                    anim.SetBool("Moving", true);
                }   
            }
            
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
        if(collision.gameObject.tag == "PlayerProjectile" && alive == true)
        {
            hitSource.Play();
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            rb.detectCollisions = false;
            GameManager.instance.score+=5;
            deathSource.Play();
            anim.SetTrigger("Die");
            alive = false;
            GetComponentInChildren<SkinnedMeshRenderer>().material = visible;
        }
    }

    public void Die()
    {
        StartCoroutine(StartDestroy());
    }

    IEnumerator StartDestroy()
    {
        yield return StartCoroutine(Destroy());
        Vector3 spawn = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
        Instantiate(spawner, spawn, Quaternion.identity);
        Instantiate(deathBurst, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    IEnumerator Destroy()
    {
        CanvasManager canvas = FindObjectOfType<CanvasManager>();
        canvas.gRef.Remove(this);

        if (GameManager.save == true)
        {
            List<LoadSaveManager.GameStateData.DataEnemy> enemies =
            GameManager.StateManager.gameState.enemies;

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].enemyID == enemyID)
                {
                    // Found enemy. Now break break from loop
                    GameManager.StateManager.gameState.enemies.Remove(enemies[i]);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.2f);
    }
}
