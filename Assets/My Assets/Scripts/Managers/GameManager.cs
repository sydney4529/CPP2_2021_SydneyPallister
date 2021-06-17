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
    public static bool vulnerable = true;

    public static bool save;

    AsyncOperation async;
    public static GameManager instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    public static LoadSaveManager StateManager
    {
        get
        {
            if (!statemanager)
                statemanager = instance.GetComponent<LoadSaveManager>();

            return statemanager;
        }
    }

    // Internal reference to single active instance of object - for singleton behaviour
    //private static GameManager instance = null;

    private static LoadSaveManager statemanager = null;

    private static bool bShouldLoad = false;

    int _score;
    public int score
    {
        get { return _score; }
        set
        {
            _score = value;
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

            if(_health <= 0 && alive == true)
            {
                playerInstance.GetComponent<PlayerCollision>().deathSource.Play();
                alive = false;
                IsInputEnabled = false;
                lives--;
                playerInstance.GetComponent<PlayerCollision>().anim.SetTrigger("Die");
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
                    if (alive == false)
                    {
                        //playerInstance.GetComponent<PlayerCollision>().anim.SetTrigger("Die");
                    }
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

        }
    }

    // Use this for initialization
    void Start()
    {
        if (instance)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        // Check if the Level should be loaded
        if (bShouldLoad)
        {
            // Load the file to read from	
            StateManager.Load(Application.persistentDataPath + "/SaveGame.xml");

            // Reset load flag
            bShouldLoad = false;
        }

        // Store the Player name, which can come from an InputField UI Component
        PlayerPrefs.SetString("Name", "Dragon");
        PlayerPrefs.SetFloat("Volume", 0.5f);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update()
    {

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

            if (save)
            {
                SceneManager.LoadScene("MainScene");
                CanvasManager canvasManager = FindObjectOfType<CanvasManager>();
                canvasManager.cRef = FindObjectOfType<PlayerSave>();
                alive = true;
                vulnerable = true;
                IsInputEnabled = true;
                canvasManager.LoadGame();  
            }
            else
            {
                SceneManager.LoadScene("MainScene");
                health = 3;
                alive = true;
                vulnerable = true;
                IsInputEnabled = true;

            }

        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
        lives = maxLives;
        health = maxHealth;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
        Cursor.lockState = CursorLockMode.None;
        score = 0;
        save = false;
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
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
        IsInputEnabled = true;
        alive = true;
    }

    public void Save()
    {
        save = true;

        // Call save game functionality
        StateManager.Save(Application.persistentDataPath + "/SaveGame.xml");
    }

    public void Load()
    {
        bShouldLoad = true;

        //Call load game functionality
        StateManager.Load(Application.persistentDataPath + "/SaveGame.xml");
    }

    public void PreLoadGame()
    {
        async = SceneManager.LoadSceneAsync("MainScene");
        async.allowSceneActivation = false;
        StartCoroutine(StartLevel());

    }

    IEnumerator StartLevel()
    {
        while (!async.isDone)
        {
            if (async.progress == 0.9f)
            {
                yield return new WaitForSeconds(1.0f);
                async.allowSceneActivation = true;
            }
            yield return 0;
        }

        alive = true;
        vulnerable = true;
        IsInputEnabled = true;

        if (save)
        {
            playerInstance.GetComponent<PlayerSave>().LoadGameComplete();
            Load();
        }
        else
        {
            score = 0;
            health = 3;
        }
    }  
}
