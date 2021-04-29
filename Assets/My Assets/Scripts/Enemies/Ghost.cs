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

    // Start is called before the first frame update
    void Start()
    {
        if(speed <= 0)
        {
            speed = 4f;
        }

        rb = GetComponent<Rigidbody>();
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
            Destroy(gameObject);
        }
    }
}
