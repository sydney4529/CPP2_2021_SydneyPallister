using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public LevelManager currentLevel;

    public GameObject player;

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


    public static GameManager instance
    {
        get { return _instance; }
        set { _instance = value; }
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

    public void StartGame()
    {

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

    }

}
