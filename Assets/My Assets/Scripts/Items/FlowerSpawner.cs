using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    Animator anim;
    public GameObject itemSpawner;
    bool opened;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("PlayerProjectile"))
        {
            if(!opened)
            {
                anim.SetTrigger("Open");
                opened = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {
        Vector3 spawn = new Vector3(transform.position.x + 0.3f, transform.position.y + 5, transform.position.z - 1.5f);
        Instantiate(itemSpawner, spawn, Quaternion.identity);
    }
}
