using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public LevelManager currentLevel;

    public GameObject player;
    public GameObject playerInstance;

    public static bool IsInputEnabled = true;
    public static bool alive = true;

    public static GameManager instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    int _score;
    public int score
    {
        get { return _score; }
        set
        {
            _score = value;
            //Debug.Log("Current score is: " + _score);
        }
    }

    public int maxHealth = 3;
    int _health;

    public int health
    {
        get { return _health; }
        set
        {
            _health = value;

            if(_health > maxHealth)
            {
                _health = maxHealth; 
            }

            if(_health <= 0)
            {
                //Destroy(playerInstance);
                alive = false;
                IsInputEnabled = false;
                lives--;
            }
        }
    }

    public int maxLives = 3;
    int _lives;

    public int lives
    {
        get { return _lives; }
        set
        {
            if (_lives > value)
            {
                if (SceneManager.GetActiveScene().name == "MainScene")
                {

                    //Debug.Log("Should have spawned");
                    playerInstance.GetComponent<PlayerCollision>().anim.SetTrigger("Die");
                    //Respawn();
                    

                }
            }

            _lives = value;

            if (_lives > maxLives)
            {
                _lives = maxLives;
            }
            else if (_lives <= 0)
            {
                _lives = 0;
                if (SceneManager.GetActiveScene().name == "MainScene")
                {
                    //SceneManager.LoadScene("GameOver");
                }
            }
            //Debug.Log(_lives);

        }
    }

    // Start is called before the first frame update
    void Start()
    {

        if (instance)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //if (player)

            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    if (SceneManager.GetActiveScene().name == "Jungle_Hijinx")
            //    {
            //        SceneManager.LoadScene("TitleScreen");
            //    }
            //    else if (SceneManager.GetActiveScene().name == "TitleScreen")
            //    {
            //        SceneManager.LoadScene("Jungle_Hijinx");
            //    }
            //    else if (SceneManager.GetActiveScene().name == "GameOver")
            //    {
            //        SceneManager.LoadScene("TitleScreen");
            //    }

            //}

            if (Input.GetKeyDown(KeyCode.Backspace))
            {

#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
            }
    }


    public void SpawnPlayer(Transform spawnLocation)
    {
        CinemachineFreeLook thirdPersonCamera = GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<CinemachineFreeLook>();
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Ghost[] ghostEnemy = FindObjectsOfType<Ghost>();

        if (thirdPersonCamera)
        {

            playerInstance = Instantiate(player, spawnLocation.position, spawnLocation.rotation);
            playerInstance.GetComponent<ThirdPersonMovement>().cam = mainCamera.transform;

            thirdPersonCamera.Follow = playerInstance.transform;
            thirdPersonCamera.LookAt = playerInstance.transform;

            for (int i = 0; i < ghostEnemy.Length; i++)
            {
                ghostEnemy[i].player = playerInstance.transform;
            }

        }
        else
        {
            SpawnPlayer(spawnLocation);
        }

    }

    public void Respawn()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            Destroy(playerInstance);
            //playerInstance.transform.position = currentLevel.spawnLocation.position;
            health = 3;
            alive = true;
            IsInputEnabled = true;
            SpawnPlayer(currentLevel.spawnLocation);
            
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
        Cursor.lockState = CursorLockMode.None;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        IsInputEnabled = true;
        alive = true;
    }

}
