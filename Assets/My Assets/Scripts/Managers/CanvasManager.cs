using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    // Start is called before the first frame update
    void Start()
    {
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
            returnToGame.onClick.AddListener(() => ReturnToGame());
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
    }

    // Update is called once per frame
    void Update()
    {

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
                //pauseMenu.SetActive(!pauseMenu.activeSelf);
                //Time.timeScale = 0;
                //if (pauseMenu.activeSelf || soundMenu.activeSelf)
                //{
                //    //PauseGame();
                //    ReturnToGame();
                //}
                if (pauseMenu.activeSelf)
                {
                    //PauseGame();
                    ReturnToGame();
                }
                else if (!pauseMenu.activeSelf)
                {
                    //ReturnToGame();
                    PauseGame();
                    //pauseAudio.Play();
                }
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
        pauseMenu.SetActive(true);
        GameManager.IsInputEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        //pauseAudio.Play();
        Time.timeScale = 0f;
    }

    public void ReturnToGame()
    {
        Time.timeScale = 1f;
        GameManager.IsInputEnabled = true;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        //pauseAudio.Play();
        //if (soundMenu.activeSelf)
        //{
        //    soundMenu.SetActive(false);
        //}
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
        //muteText.text = "Muted : " + muteToggle.isOn;
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
