using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    Rigidbody rb;

    public GameObject target;

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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
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

        

    }

    // Update is called once per frame
    void Update()
    {

        //if (health <= 0 && alive)
        //{
        //    enemyType = EnemyType.Dead;
        //    target = null;
        //    agent.velocity = new Vector3(0, 0, 0);
        //    anim.SetTrigger("Die");
        //    alive = false;
        //}


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
                //if(co != null)
                //{
                //    StopCoroutine(co);
                //} 
            }

            if (Vector3.Distance(transform.position, GameManager.instance.playerInstance.transform.position) <= attackDist && !fired)
            {
                co = StartCoroutine(DelayFire());
                //StartCoroutine(DelayFire());
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

        Debug.Log(health);
        Debug.Log(target);
        Debug.Log(alive);
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
                alive = false;
                enemyType = EnemyType.Dead;
                target = null;
                anim.SetTrigger("Die");
            }
            
            //Hurt();
        }
    }

    void DryadFire()
    {
        Projectile projectileInstance = Instantiate(bullet, spawnPoint.position, Quaternion.identity);
        projectileInstance.GetComponent<Rigidbody>().velocity = transform.forward * projectileInstance.speed;
        projectileInstance.transform.rotation = transform.rotation;
    }

    IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(2);
        anim.SetTrigger("Attack");
        fired = false;
    }

    void DryadDie()
    {
        Vector3 spawn = new Vector3(transform.position.x, transform.position.y+5, transform.position.z);

        Instantiate(spawner, spawn, Quaternion.identity);
        Destroy(gameObject);
    }
}
