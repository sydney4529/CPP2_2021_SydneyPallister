using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_ProximityBased : MonoBehaviour
{
    public float slowDown;

    // Start is called before the first frame update
    void Start()
    {
        if(slowDown <= 0)
        {
            slowDown = 10.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
                ThirdPersonMovement player = other.GetComponent<ThirdPersonMovement>();
                if (player)
                {
                player.speed -= slowDown;
                }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ThirdPersonMovement player = other.GetComponent<ThirdPersonMovement>();
        if (player)
        {
            player.speed += slowDown;
        }
    }
}
