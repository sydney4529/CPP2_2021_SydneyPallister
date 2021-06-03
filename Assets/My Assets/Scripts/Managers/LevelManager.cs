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
        if(GameManager.save == false)
        {
            if(GameManager.instance.lives == 0)
            {
                GameManager.instance.lives = startingLives;
            }
            
            GameManager.instance.health = startingHealth;
        }

        if(GameManager.save == true)
        {
            Vector3 newPos = new Vector3(GameManager.StateManager.gameState.player.posRotScale.posX, GameManager.StateManager.gameState.player.posRotScale.posY, GameManager.StateManager.gameState.player.posRotScale.posZ);
            spawnLocation.transform.position = newPos;
        }
        
        GameManager.instance.SpawnPlayer(spawnLocation);
        GameManager.instance.currentLevel = gameObject.GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
