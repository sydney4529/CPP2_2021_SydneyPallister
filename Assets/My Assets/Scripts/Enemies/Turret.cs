using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Turret : MonoBehaviour
{
    [Header ("Projectile Vars")]
    public Rigidbody bullet;
    public Transform spawnPoint;
    public Transform player;

    [Header("Death Prefabs")]
    public GameObject spawner;
    public ParticleSystem deathBurst;

    [Header("Stats")]
    public int health;
    public string enemyID;

    Coroutine co;

    Rigidbody rb;
    Animator anim;

    float attackDist;

    bool canFire;
    bool alive;
    bool done;

    public AudioClip enemyDeath;
    public AudioClip enemyHit;
    public AudioClip fire;
    public AudioMixerGroup mixerGroup;
    AudioSource deathSource;
    AudioSource fireSource;
    AudioSource hitSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        enemyID = GetComponent<UniqueId>().uniqueId;

        attackDist = 50f;
        health = 2;
        alive = true;
        canFire = true;

        name = "Turret";

        if (GameManager.save == true)
        {
            Debug.Log("save is true");
            LoadGameComplete();
        }
    }

    public void SaveGamePrepare()
    {
        List<LoadSaveManager.GameStateData.DataEnemy> enemies2 =
            GameManager.StateManager.gameState.enemies2;

        for (int i = 0; i < enemies2.Count; i++)
        {
            if (enemies2[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                GameManager.StateManager.gameState.enemies2.Remove(enemies2[i]);
                break;
            }
        }

        LoadSaveManager.GameStateData.DataEnemy data = new LoadSaveManager.GameStateData.DataEnemy();

        data.enemyID = GetComponent<UniqueId>().uniqueId;
        data.saveAlive = alive;
        data.health = health;

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
        GameManager.StateManager.gameState.enemies2.Add(data);
    }

    public void LoadGameComplete()
    {
        // Cycle through enemies and find matching ID
        List<LoadSaveManager.GameStateData.DataEnemy> enemies2 =
            GameManager.StateManager.gameState.enemies2;

        // Reference to this enemy
        LoadSaveManager.GameStateData.DataEnemy data = null;

        for (int i = 0; i < enemies2.Count; i++)
        {
            if (enemies2[i].enemyID == enemyID)
            {
                // Found enemy. Now break break from loop
                data = enemies2[i];
                break;
            }
        }

        // If here and no enemy is found, then it was destroyed when saved. So destroy.
        if (data == null)
        {
            CanvasManager canvas = FindObjectOfType<CanvasManager>();
            canvas.TRef.Remove(this);
            Destroy(gameObject);
            return;
        }

        // Else load enemy data
        enemyID = data.enemyID;
        alive = data.saveAlive;
        health = data.health;

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
    void Update()
    {
        if (!deathSource)
        {
            deathSource = gameObject.AddComponent<AudioSource>();
            deathSource.outputAudioMixerGroup = mixerGroup;
            deathSource.clip = enemyDeath;
            deathSource.loop = false;
        }
        if (!fireSource)
        {
            fireSource = gameObject.AddComponent<AudioSource>();
            fireSource.outputAudioMixerGroup = mixerGroup;
            fireSource.clip = fire;
            fireSource.loop = false;
        }
        if (!hitSource)
        {
            hitSource = gameObject.AddComponent<AudioSource>();
            hitSource.outputAudioMixerGroup = mixerGroup;
            hitSource.clip = enemyHit;
            hitSource.loop = false;
        }

        if (player == null)
        {
            player = FindObjectOfType<ThirdPersonMovement>().transform;
        }

        if (Vector3.Distance(transform.position, player.position) <= attackDist)
        {
            transform.LookAt(player);
            if (canFire == true)
            {
                co = StartCoroutine(Attack());
            }
        }
        else
        {
            if(co != null)
            {
                StopCoroutine(co);
                canFire = true;
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile")
        {
            if (health > 1 && GameManager.instance.playerInstance.GetComponent<PlayerFire>().poweredUp == false)
            {
                hitSource.Play();
                anim.SetTrigger("Hurt");
                health--;
            }
            else
            {
                if (alive == true)
                {
                    rb.velocity = Vector3.zero;
                    rb.useGravity = false;
                    rb.detectCollisions = false;
                    rb.constraints = ~RigidbodyConstraints.FreezePositionY;
                    rb.useGravity = true;
                    alive = false;
                    if (co != null)
                    {
                        StopCoroutine(co);
                    }
                    deathSource.Play();
                    anim.SetTrigger("Die");
                    CanvasManager canvas = FindObjectOfType<CanvasManager>();
                    canvas.TRef.Remove(this);
                    List<LoadSaveManager.GameStateData.DataEnemy> enemies2 =
                    GameManager.StateManager.gameState.enemies2;

                    for (int i = 0; i < enemies2.Count; i++)
                    {
                        if (enemies2[i].enemyID == enemyID)
                        {
                            // Found enemy. Now break break from loop
                            GameManager.StateManager.gameState.enemies2.Remove(enemies2[i]);
                            break;
                        }
                    }
                }
            }
                
        }
    }

    IEnumerator Attack()
    {
        canFire = false;
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));

        anim.SetTrigger("Attack");
        Rigidbody temp = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
        temp.AddForce(spawnPoint.forward * 60.0f, ForceMode.Impulse);
        fireSource.Play();

        yield return new WaitForSeconds(Random.Range(2.0f, 3.5f));
        canFire = true;
    }

    public void Die()
    {
        Vector3 spawn = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
        Instantiate(spawner, spawn, Quaternion.identity);
        Instantiate(deathBurst, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
