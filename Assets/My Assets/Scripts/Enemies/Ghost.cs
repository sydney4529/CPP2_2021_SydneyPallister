using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Camera mainCamera;
    public Transform player;
    public float speed;
    public Material invisible;
    public Material visible;

    float MinDist = 60f;

    Rigidbody rb;

    public string enemyID;

    public bool alive;

    // Start is called before the first frame update
    void Start()
    {
        if(speed <= 0)
        {
            speed = 4f;
        }

        name = "Ghost";

        alive = true;

        //enemyID = GetInstanceID();

        enemyID = GetComponent<UniqueId>().uniqueId;

        rb = GetComponent<Rigidbody>();

        if(GameManager.save == true)
        {
            LoadGameComplete();
        }
    }

    public void SaveGamePrepare()
    {
        List<LoadSaveManager.GameStateData.DataEnemy> enemies =
            GameManager.StateManager.gameState.enemies;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                GameManager.StateManager.gameState.enemies.Remove(enemies[i]);
                break;
            }
        }

        //GameManager.StateManager.gameState.enemies.Remove(this);
        LoadSaveManager.GameStateData.DataEnemy data = new LoadSaveManager.GameStateData.DataEnemy();

        //data.health = health;
        data.enemyID = GetComponent<UniqueId>().uniqueId;
        data.saveAlive = alive;

        data.posRotScale.posX = transform.position.x;
        data.posRotScale.posY = transform.position.y;
        data.posRotScale.posZ = transform.position.z;

        data.posRotScale.rotX = transform.localEulerAngles.x;
        data.posRotScale.rotY = transform.localEulerAngles.y;
        data.posRotScale.rotZ = transform.localEulerAngles.z;

        data.posRotScale.scaleX = transform.localScale.x;
        data.posRotScale.scaleY = transform.localScale.y;
        data.posRotScale.scaleZ = transform.localScale.z;

        //Add enemy to Game State
        GameManager.StateManager.gameState.enemies.Add(data);

        Debug.Log(GameManager.StateManager.gameState.enemies.Count);
    }

    public void LoadGameComplete()
    {
        // Cycle through enemies and find matching ID
        List<LoadSaveManager.GameStateData.DataEnemy> enemies =
            GameManager.StateManager.gameState.enemies;

        // Reference to this enemy
        LoadSaveManager.GameStateData.DataEnemy data = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                data = enemies[i];
                break;
            }
        }

        // If here and no enemy is found, then it was destroyed when saved. So destroy.
        if (data == null)
        {
            CanvasManager canvas = FindObjectOfType<CanvasManager>();
            canvas.gRef.Remove(this);
            Destroy(gameObject);
            return;
        }

        // Else load enemy data
        enemyID = data.enemyID;
        alive = data.saveAlive;
        //health = data.health;

        // Set position
        transform.position = new Vector3(data.posRotScale.posX,
            data.posRotScale.posY, data.posRotScale.posZ);

        // Set rotation
        transform.localRotation = Quaternion.Euler(data.posRotScale.rotX,
            data.posRotScale.rotY, data.posRotScale.rotZ);

        // Set scale
        transform.localScale = new Vector3(data.posRotScale.scaleX,
            data.posRotScale.scaleY, data.posRotScale.scaleZ);

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float dot = Vector3.Dot(player.forward, (transform.position - player.position).normalized);

        if (dot > 0.7f) 
        {
            if(Vector3.Distance(transform.position, player.position) <= MinDist)
            {
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, player.transform.rotation, 2.5f);
                GetComponentInChildren<SkinnedMeshRenderer>().material = invisible;
            }
        }
        else
        {
            if(Vector3.Distance(transform.position, player.position) <= MinDist)
            {
                Vector3 pos = Vector3.MoveTowards(transform.position, player.position, speed * Time.fixedDeltaTime);
                rb.MovePosition(pos);
                transform.LookAt(player);
                GetComponentInChildren<SkinnedMeshRenderer>().material = visible;
            }
            
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile")
        {
            GameManager.instance.score+=5;
            //    List<LoadSaveManager.GameStateData.DataEnemy> enemies =
            //GameManager.StateManager.gameState.enemies;
            //    for (int i = 0; i < enemies.Count; i++)
            //    {
            //        if (enemies[i].enemyID == enemyID)
            //        {
            //            // Found enemy. Now break break from loop
            //            enemies[i].saveAlive = false;
            //            break;
            //        }
            //    }
            //CanvasManager canvas = FindObjectOfType<CanvasManager>();
            //canvas.gRef.Remove(this);
            //Destroy(gameObject);
            StartCoroutine(StartDestroy());
        }
    }

    IEnumerator StartDestroy()
    {

        yield return StartCoroutine(Destroy());
        Destroy(gameObject);
    }

    IEnumerator Destroy()
    {
        CanvasManager canvas = FindObjectOfType<CanvasManager>();
        canvas.gRef.Remove(this);

        List<LoadSaveManager.GameStateData.DataEnemy> enemies =
            GameManager.StateManager.gameState.enemies;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                GameManager.StateManager.gameState.enemies.Remove(enemies[i]);
                break;
            }
        }

        yield return true;
    }
}
