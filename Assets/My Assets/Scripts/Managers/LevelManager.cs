using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int startingLives;
    public int startingHealth;

    public Transform spawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.instance.currentLevel = GetComponent<LevelManager>();
        GameManager.instance.lives = startingLives;
        GameManager.instance.health = startingHealth;
        GameManager.instance.SpawnPlayer(spawnLocation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
