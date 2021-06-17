using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    public int health;
    public float score;

    public GameObject mainCam;

    // Start is called before the first frame update
    void Start()
    {
        // Check if Player name is stored
        if (PlayerPrefs.HasKey("Name"))
            name = PlayerPrefs.GetString("Name");
        else
            name = "Default";
    }

    private void Update()
    {
        if(!mainCam)
        {
            mainCam = GameObject.FindGameObjectWithTag("ThirdPersonCamera");
        }
    }

    public void SaveGamePrepare()
    {
        // Get Player Data Object
        LoadSaveManager.GameStateData.DataPlayer data =
            GameManager.StateManager.gameState.player;

        // Fill in player data for save game
        data.collectedScore = GameManager.instance.score;
        data.health = GameManager.instance.health;

        data.posRotScale.posX = transform.position.x;
        data.posRotScale.posY = transform.position.y;
        data.posRotScale.posZ = transform.position.z;

        data.posRotScale.rotX = transform.localEulerAngles.x;
        data.posRotScale.rotY = transform.localEulerAngles.y;
        data.posRotScale.rotZ = transform.localEulerAngles.z;

        data.posRotScale.scaleX = transform.localScale.x;
        data.posRotScale.scaleY = transform.localScale.y;
        data.posRotScale.scaleZ = transform.localScale.z;

    }

    public void LoadGameComplete()
    {
        // Get Player Data Object
        LoadSaveManager.GameStateData.DataPlayer data =
            GameManager.StateManager.gameState.player;

        // Load data back to Player
        GameManager.instance.health = data.health;
        GameManager.instance.score = (int)data.collectedScore;

        // Give player weapon, activate and destroy weapon power-up
        //if (data.collectedWeapon)
        //{
        //    // Find weapon powerup in level
        //    GameObject weaponPowerUp = GameObject.Find("Weapon_PowerUp");

        //    // Send OnTriggerEnter message
        //    weaponPowerUp.SendMessage("OnTriggerEnter2D", GetComponent<Collider2D>(),
        //        SendMessageOptions.DontRequireReceiver);

        //}


        //StartCoroutine(Wait(data));

        //Set position
        transform.position = new Vector3(data.posRotScale.posX,
            data.posRotScale.posY, data.posRotScale.posZ);

        // Set rotation
        transform.localRotation = Quaternion.Euler(data.posRotScale.rotX,
            data.posRotScale.rotY, data.posRotScale.rotZ);

        // Set scale
        transform.localScale = new Vector3(data.posRotScale.scaleX,
            data.posRotScale.scaleY, data.posRotScale.scaleZ);

    }


    IEnumerator Wait(LoadSaveManager.GameStateData.DataPlayer data)
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
            yield return StartCoroutine(SaveMe(data));
            Time.timeScale = 0;
        }
        else
        {
            yield return StartCoroutine(SaveMe(data));
        }
        //transform.position = new Vector3(data.posRotScale.posX,
        //    data.posRotScale.posY, data.posRotScale.posZ);

        //// Set rotation
        //transform.localRotation = Quaternion.Euler(data.posRotScale.rotX,
        //    data.posRotScale.rotY, data.posRotScale.rotZ);

        //// Set scale
        //transform.localScale = new Vector3(data.posRotScale.scaleX,
        //    data.posRotScale.scaleY, data.posRotScale.scaleZ);
        //yield return new WaitForSeconds(0.001f);
        

    }

    IEnumerator SaveMe(LoadSaveManager.GameStateData.DataPlayer data)
    {
        transform.position = new Vector3(data.posRotScale.posX,
            data.posRotScale.posY, data.posRotScale.posZ);

        // Set rotation
        transform.localRotation = Quaternion.Euler(data.posRotScale.rotX,
            data.posRotScale.rotY, data.posRotScale.rotZ);

        // Set scale
        transform.localScale = new Vector3(data.posRotScale.scaleX,
            data.posRotScale.scaleY, data.posRotScale.scaleZ);
        yield return true;
    }
}
