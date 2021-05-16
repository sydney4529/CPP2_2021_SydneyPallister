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

    enum EnemyType { Chase, Patrol}
    [SerializeField] EnemyType enemyType;

    enum PatrolType { DistanceBased, TriggerBased }
    [SerializeField] PatrolType patrolType;

    public bool autoGenPath;
    public string pathName;

    public GameObject[] path;

    public int pathIndex;

    public float distanceToNextNode;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

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
        if (target && enemyType == EnemyType.Patrol && patrolType == PatrolType.DistanceBased)
        {
            //Debug.Log(Vector3.Distance(transform.position, target.transform.position)); ;

            Debug.DrawLine(transform.position, target.transform.position, Color.red);

            //if((transform.position - target.transform.position).magnitude < distanceToNextNode)

            if(agent.remainingDistance < distanceToNextNode)
            {
                if(path.Length > 0)
                {
                    pathIndex++;

                    pathIndex %= path.Length;

                    if (pathIndex >= path.Length)
                        pathIndex = 0;

                    target = path[pathIndex];
                }
            }
        }


        SetTarget();
        //Debug.Log(target);

        if (target)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    void SetTarget()
    {
        if (!target && enemyType == EnemyType.Chase)
        {
            target = GameObject.FindWithTag("Player");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
