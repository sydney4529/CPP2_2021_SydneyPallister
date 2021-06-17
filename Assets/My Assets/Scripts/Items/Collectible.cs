using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Collectible : MonoBehaviour
{
    public enum CollectibleType
    {
        FIREPOWERUP,
        SPEEDPOWERUP,
        SHEILDPOWERUP,
        COIN,
        GEM1,
        GEM2,
        LIFE,
        HEALTH,
        END
    }

    public ParticleSystem collect;

    public CollectibleType currentCollectible;

    public float rotSpeed;
    bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotSpeed * Time.deltaTime, 0); //rotates 50 degrees per second around z axis
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player" && isActive)
        {
            isActive = false;
            Instantiate(collect, transform.position, transform.rotation);
            Destroy(gameObject);

            switch (currentCollectible)
            {

                case CollectibleType.FIREPOWERUP:
                    collision.gameObject.GetComponent<PlayerFire>().poweredUp = true;
                    break;

                case CollectibleType.SPEEDPOWERUP:
                    collision.gameObject.GetComponent<ThirdPersonMovement>().speedPowered = true;
                    break;

                case CollectibleType.SHEILDPOWERUP:
                    collision.gameObject.GetComponentInChildren<PlayerMaterials>().shieldPowered = true;
                    break;

                case CollectibleType.LIFE:
                    GameManager.instance.lives++;
                    break;

                case CollectibleType.HEALTH:
                    GameManager.instance.health++;
                    break;

                case CollectibleType.GEM1:
                    GameManager.instance.score+=5;
                    break;

                case CollectibleType.GEM2:
                    GameManager.instance.score+=10;
                    break;

                case CollectibleType.COIN:
                    GameManager.instance.score += 15;
                    break;

                case CollectibleType.END:
                    GameManager.instance.score += 50;
                    break;

            }
        }
    }
}
