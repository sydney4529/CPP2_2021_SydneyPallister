using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_TimeBased : MonoBehaviour
{
    public int damage;
    public float damageTime;
    float timeSinceLastDamange;

    // Start is called before the first frame update
    void Start()
    {
        if(damage <= 0)
        {
            damage = 1;
        }

        if (damageTime <= 0)
        {
            damageTime = 2.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(Time.time > timeSinceLastDamange + damageTime)
            {
                PlayerCollision player = other.GetComponent<PlayerCollision>();
                if(player)
                {
                    player.ApplyDamage(damage);
                }

                timeSinceLastDamange = Time.time;
            }
        }
    }
}
