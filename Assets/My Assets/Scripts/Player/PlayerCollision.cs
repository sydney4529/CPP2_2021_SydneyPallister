using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    ThirdPersonMovement movementScript;
    bool notTriggered;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<ThirdPersonMovement>();
        anim = GetComponent<Animator>();
        notTriggered = true;
    }

    // Update is called once per frame
    void Update()
    {
        CanvasManager canvas = FindObjectOfType<CanvasManager>();
        Debug.Log(canvas.gRef.Count);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.alive == true && GameManager.vulnerable)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                GameManager.instance.health--;
                anim.SetTrigger("Hurt");
            }

            if (collision.gameObject.tag == "EnemyProjectile")
            {
                GameManager.instance.health--;
                anim.SetTrigger("Hurt");
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(GameManager.alive == true && GameManager.vulnerable)
        {
            if (other.gameObject.tag == "Vines")
            {
                movementScript.speed = 10;
                movementScript.isRunning = false;
                if (notTriggered)
                {
                    StartCoroutine(Wait());
                    notTriggered = false;
                }
            }

            if (other.gameObject.tag == "Water")
            {
                GameManager.instance.health -= 3;
            }

            if (other.gameObject.tag == "Blockade")
            {
                if (notTriggered)
                {
                    StartCoroutine(Wait());
                    notTriggered = false;
                }
            }
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Vines")
        {
            movementScript.speed = 20;
        }
    }

    IEnumerator Wait()
    {
        GameManager.instance.health--;
        anim.SetTrigger("Hurt");
        yield return new WaitForSeconds(2);
        notTriggered = true;
    }

    public void RespawnDragon()
    {
        if (GameManager.instance.lives > 0)
        {
            //GameManager.instance.Respawn();
            GameManager.instance.PreLoadGame();
        }
        else
        {
            GameManager.instance.GameOver();
        }

        //GameManager.instance.Respawn();

    }

    public void ApplyDamage(int damage)
    {
        GameManager.instance.health-=damage;
    }
}
