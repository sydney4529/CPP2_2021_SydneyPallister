using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Collectible : MonoBehaviour
{
    public enum CollectibleType
    {
        POWERUP,
        COIN,
        GEM1,
        GEM2,
        LIFE,
        END
    }

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
            Destroy(gameObject);
            //input do something code here
            switch (currentCollectible)
            {

                case CollectibleType.POWERUP:

                    break;

                case CollectibleType.LIFE:

                    break;

                case CollectibleType.GEM1:

                    break;

                case CollectibleType.GEM2:

                    break;

                case CollectibleType.COIN:

                    break;

                case CollectibleType.END:

                    break;

            }
        }
    }
}
