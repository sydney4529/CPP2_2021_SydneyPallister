using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    Rigidbody rb;

    public GameObject target;
    public ParticleSystem deathBurst;

    enum EnemyType { Chase, Patrol, Dead}
    [SerializeField] EnemyType enemyType;

    enum PatrolType { DistanceBased, TriggerBased }
    [SerializeField] PatrolType patrolType;

    public bool autoGenPath;
    public string pathName;

    public GameObject[] path;
    public Projectile bullet;
    public Transform spawnPoint;
    public GameObject spawner;
    Coroutine co;

    public int pathIndex;

    public float distanceToNextNode;

    float chaseDist;
    float attackDist;

    bool fired;
    bool alive;

    public int health;
    public string enemyID;

    public AudioClip enemyDeath;
    public AudioClip enemyHit;
    public AudioClip fire;
    public AudioMixerGroup mixerGroup;
    AudioSource deathSource;
    AudioSource fireSource;
    AudioSource hitSource;

    // Start is called before the first frame update
    void Start()
    {
        name = "Dryad";

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyID = GetComponent<UniqueId>().uniqueId;

        chaseDist = 30;
        attackDist = 20;
        health = 3;
        alive = true;

        anim.applyRootMotion = false;

        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (string.IsNullOrEmpty(pathName))
        {
            pathName = "PatrolNode";
        }

        if (!target && enemyType == EnemyType.Chase)
        {
            target = GameObject.FindWithTag("Player");
        }
        else if(enemyType == EnemyType.Patrol)
        {
            if(autoGenPath)
            {
                path = GameObject.FindGameObjectsWithTag(pathName);
            }

            if(path.Length > 0)
            {
                target = path[pathIndex];
            }
        }

        if(distanceToNextNode <= 0)
        {
            distanceToNextNode = 1.0f;
        }

        if(target)
        {
            agent.SetDestination(target.transform.position);
        }

        if (GameManager.save == true)
        {
            //Debug.Log("save is true");
            LoadGameComplete();
        }

    }

    public void SaveGamePrepare()
    {
        List<LoadSaveManager.GameStateData.DataEnemy> enemies1 =
            GameManager.StateManager.gameState.enemies1;

        for (int i = 0; i < enemies1.Count; i++)
        {
            if (enemies1[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                GameManager.StateManager.gameState.enemies1.Remove(enemies1[i]);
                break;
            }
        }

        //GameManager.StateManager.gameState.enemies1.Clear();
        LoadSaveManager.GameStateData.DataEnemy data = new LoadSaveManager.GameStateData.DataEnemy();

        data.enemyID = GetComponent<UniqueId>().uniqueId;
        data.health = health;
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
        GameManager.StateManager.gameState.enemies1.Add(data);
    }

    public void LoadGameComplete()
    {

        // Cycle through enemies and find matching ID
        List<LoadSaveManager.GameStateData.DataEnemy> enemies1 =
            GameManager.StateManager.gameState.enemies1;

        // Reference to this enemy
        LoadSaveManager.GameStateData.DataEnemy data = null;

        for (int i = 0; i < enemies1.Count; i++)
        {
            if (enemies1[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                data = enemies1[i];
                break;
            }
        }

        // If here and no enemy is found, then it was destroyed when saved. So destroy.
        if (data == null)
        {
            CanvasManager canvas = FindObjectOfType<CanvasManager>();
            canvas.eRef.Remove(this);
            Destroy(gameObject);
            return;
        }

        // Else load enemy data
        enemyID = data.enemyID;
        health = data.health;
        alive = data.saveAlive;
        //Debug.Log(data.saveAlive);

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
    void Update()
    {
        //Debug.Log(GameManager.StateManager.gameState.enemies1.Count);

        if (!deathSource)
        {
            deathSource = gameObject.AddComponent<AudioSource>();
            deathSource.outputAudioMixerGroup = mixerGroup;
            deathSource.clip = enemyDeath;
            deathSource.loop = false;
            //dieSource.Play();
        }
        if (!fireSource)
        {
            fireSource = gameObject.AddComponent<AudioSource>();
            fireSource.outputAudioMixerGroup = mixerGroup;
            fireSource.clip = fire;
            fireSource.loop = false;
        }
        if (!hitSource)
        {
            hitSource = gameObject.AddComponent<AudioSource>();
            hitSource.outputAudioMixerGroup = mixerGroup;
            hitSource.clip = enemyHit;
            hitSource.loop = false;
        }

        if (alive)
        {

            if (target && enemyType == EnemyType.Patrol && patrolType == PatrolType.DistanceBased)
            {
                //Debug.Log(Vector3.Distance(transform.position, target.transform.position)); 

                Debug.DrawLine(transform.position, target.transform.position, Color.red);

                //if((transform.position - target.transform.position).magnitude < distanceToNextNode)

                if (agent.remainingDistance < distanceToNextNode)
                {
                    if (path.Length > 0)
                    {
                        pathIndex++;

                        pathIndex %= path.Length;

                        if (pathIndex >= path.Length)
                            pathIndex = 0;

                        target = path[pathIndex];
                    }
                }
            }

            if (Vector3.Distance(transform.position, GameManager.instance.playerInstance.transform.position) <= chaseDist)
            {
                enemyType = EnemyType.Chase;
                target = GameObject.FindWithTag("Player");
            }
            else
            {
                enemyType = EnemyType.Patrol;
                target = path[pathIndex];
            }

            if (Vector3.Distance(transform.position, GameManager.instance.playerInstance.transform.position) <= attackDist && !fired)
            {
                co = StartCoroutine(DelayFire());
                fired = true;
            }

            SetTarget();

            if (target)
            {
                agent.SetDestination(target.transform.position);
            }
        }
        else
        {
            agent.velocity = Vector3.zero;
            if (co != null)
            {
                StopCoroutine(co);
            }
        }

        anim.SetFloat("Speed", transform.InverseTransformDirection(agent.velocity).z);
    }

    void SetTarget()
    {
        if (!target && enemyType == EnemyType.Chase)
        {
            target = GameObject.FindWithTag("Player");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile" && alive)
        {
            if(health > 1)
            {
                hitSource.Play();
                anim.SetTrigger("Damage");
                if(GameManager.instance.playerInstance.GetComponent<PlayerFire>().poweredUp == true)
                {
                    health-=2;
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
                    rb.velocity = Vector3.zero;
                    rb.useGravity = false;
                    rb.detectCollisions = false;
                    alive = false;
                    if (co != null)
                    {
                        StopCoroutine(co);
                    }
                    enemyType = EnemyType.Dead;
                    target = null;
                    anim.SetTrigger("Die");
                    deathSource.Play();
                    CanvasManager canvas = FindObjectOfType<CanvasManager>();
                    canvas.eRef.Remove(this);
                    List<LoadSaveManager.GameStateData.DataEnemy> enemies1 =
                    GameManager.StateManager.gameState.enemies1;

                    for (int i = 0; i < enemies1.Count; i++)
                    {
                        if (enemies1[i].enemyID == enemyID)
                        {
                            // Found enemy. Now break break from loop
                            GameManager.StateManager.gameState.enemies1.Remove(enemies1[i]);
                            break;
                        }
                    }
                }

            }
            
        }
    }

    void DryadFire()
    {
        Projectile projectileInstance = Instantiate(bullet, spawnPoint.position, Quaternion.identity);
        projectileInstance.GetComponent<Rigidbody>().velocity = transform.forward * projectileInstance.speed;
        projectileInstance.transform.rotation = transform.rotation;
        fireSource.Play();
    }

    IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(1.3f);
        anim.SetTrigger("Attack");
        fired = false;
    }

    void DryadDie()
    {
        Vector3 spawn = new Vector3(transform.position.x, transform.position.y+5, transform.position.z);
        Instantiate(spawner, spawn, Quaternion.identity);
        Instantiate(deathBurst, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
