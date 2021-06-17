using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class CanvasManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button quitButton;
    public Button returnToMenu;
    public Button returnToGame;
    public Button settingsButton;
    public Button backButton;
    public Button soundMenuButton;
    public Button pauseBackButton;
    public Button saveButton;
    public Button loadButton;

    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject soundMenu;

    [Header("Text")]
    public Text livesText;
    public Text scoreText;
    public Text healthText;
    public Text volText;
    public Text muteText;

    [Header("Slider")]
    public Slider volSlider;

    [Header("Checkbox")]
    public Toggle muteToggle;

    [Header("Health Settings")]
    public Image[] hearts;
    public int numOfHearts;
    public Sprite heartFull;
    public Sprite heartEmpty;

    public PlayerSave cRef;
    public List<Enemy> eRef = new List<Enemy>();
    public List<Ghost> gRef = new List<Ghost>();
    public List<Turret> TRef = new List<Turret>();
    public List<Exploder> explodeRef = new List<Exploder>();
    public List<Chaser> chaseRef = new List<Chaser>();

    AudioSource clickSource;

    bool gamePaused;

    // Start is called before the first frame update
    void Start()
    {
        clickSource = GetComponent<AudioSource>();
        
        if (pauseMenu)
        {
            pauseMenu.SetActive(false);
        }
        if (startButton)
        {
            startButton.onClick.AddListener(() => GameManager.instance.StartGame());
        }
        if (quitButton)
        {
            quitButton.onClick.AddListener(() => GameManager.instance.QuitGame());
        }
        if (returnToGame)
        {
            returnToGame.onClick.AddListener(() => PauseGame());
        }
        if (returnToMenu)
        {
            returnToMenu.onClick.AddListener(() => GameManager.instance.ReturnToMenu());
        }
        if (backButton)
        {
            backButton.onClick.AddListener(() => ShowMainMenu());
        }
        if (settingsButton)
        {
            settingsButton.onClick.AddListener(() => ShowSettingsMenu());
        }
        if (soundMenuButton)
        {
            soundMenuButton.onClick.AddListener(() => ShowSoundMenu());
        }
        if (pauseBackButton)
        {
            pauseBackButton.onClick.AddListener(() => ShowPauseMenu());
        }
        if (saveButton)
        {
            saveButton.onClick.AddListener(() => SaveGame());
        }
        if (loadButton)
        {
            loadButton.onClick.AddListener(() => LoadGame());
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(!cRef || cRef == null)
        {
            if(SceneManager.GetActiveScene().name == "MainScene")
            {
                cRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSave>();
            }
        }

        if (pauseMenu)
        {
            for (int i = 0; i < hearts.Length; i++)
            {

                if(i < GameManager.instance.health)
                {
                    hearts[i].sprite = heartFull;
                }
                else
                {
                    hearts[i].sprite = heartEmpty;
                }

                if(i < numOfHearts)
                {
                    hearts[i].enabled = true;
                }
                else
                {
                    hearts[i].enabled = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                PauseGame();
            }
        }
        if (livesText)
        {
            livesText.text = GameManager.instance.lives.ToString();
        }
        if (scoreText)
        {
            scoreText.text = GameManager.instance.score.ToString();
        }if (healthText)
        {
            healthText.text = GameManager.instance.health.ToString();
        }
        if (settingsMenu)
        {
            if (settingsMenu.activeSelf)
            {
                volText.text = volSlider.value.ToString();
            }
        }
        if (soundMenu)
        {
            if (soundMenu.activeSelf)
            {
                volText.text = volSlider.value.ToString();

                //muteToggle.onValueChanged.AddListener(delegate {
                //    ToggleValueChanged(muteToggle);
                //});

                if (muteToggle.isOn == true)
                {
                    muteText.text = "Muted";
                }
                else
                {
                    muteText.text = "Unmuted";
                }

            }
        }
    }

    public void PauseGame()
    {

        gamePaused = !gamePaused;
        pauseMenu.SetActive(gamePaused);

        if (gamePaused)
        {
           
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }

        GameManager.IsInputEnabled = !GameManager.IsInputEnabled;
    }

    void ShowMainMenu()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    void ShowSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    void ShowPauseMenu()
    {
        soundMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    void ShowSoundMenu()
    {
        pauseMenu.SetActive(false);
        soundMenu.SetActive(true);
    }

    void ToggleValueChanged(Toggle change)
    {
        if (muteToggle.isOn == true)
        {
            muteText.text = "Muted";
        }
        else
        {
            muteText.text = "Unmuted";
        }
    }


    public void SaveGame()
    {
        StartCoroutine(StartSave());
    }

    public void LoadGame()
    {
        GameManager.IsInputEnabled = false;
        GameManager.vulnerable = false;
        GameManager.instance.PreLoadGame();
        PauseGame();
    }

    IEnumerator StartSave()
    {
        yield return StartCoroutine(Save());
        GameManager.instance.Save();
    }

    IEnumerator Save()
    {
        if(cRef != null)
            cRef.SaveGamePrepare();

        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (fooObj.name == "Ghost")
            {
                Ghost enem = fooObj.GetComponent<Ghost>();
                gRef.Add(enem);
            }
            if (fooObj.name == "Dryad")
            {
                eRef.Add(fooObj.GetComponent<Enemy>());
            }
            if (fooObj.name == "Turret")
            {
                TRef.Add(fooObj.GetComponent<Turret>());
            }
            if (fooObj.name == "Chaser")
            {
                chaseRef.Add(fooObj.GetComponent<Chaser>());
            }
            if (fooObj.name == "Exploder")
            {
                explodeRef.Add(fooObj.GetComponent<Exploder>());
            }
        }

        foreach (Enemy enem in eRef)
        {
            if (enem != null)
            {
                enem.SaveGamePrepare();
            }
        }
        foreach (Ghost ghost in gRef)
        {
            if (ghost != null)
                ghost.SaveGamePrepare();
        }
        foreach (Turret tur in TRef)
        {
            if (tur != null)
                tur.SaveGamePrepare();
        }
        foreach (Chaser chase in chaseRef)
        {
            if (chase != null)
                chase.SaveGamePrepare();
        }
        foreach (Exploder explode in explodeRef)
        {
            if (explode != null)
                explode.SaveGamePrepare();
        }

        yield return new WaitForSeconds(1);
    }

    public void playSound()
    {
        if(isActiveAndEnabled)
            clickSource.Play();
    }

}
