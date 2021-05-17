using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3[] points;

    public int pointNumber = 0;

    public float tolerance;
    public float speed;
    public float delayTime;

    public bool automatic;

    private Vector3 currentTarget;
    private float delayStart;
    Vector3 ogScale;


    // Start is called before the first frame update
    void Start()
    {
        pointNumber = 0;

        if(points.Length > 0)
        {
            currentTarget = points[0];
        }

        tolerance = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (transform.position != currentTarget)
        {
            MovePlatform();
        }
        else
        {
            UpdateTarget();
        }

        //Debug.Log(currentTarget);
        //Debug.Log(transform.position);
    }

    private void MovePlatform()
    {
        Vector3 heading = currentTarget - transform.position;
        transform.position += (heading / heading.magnitude) * speed * Time.deltaTime;
        if(heading.magnitude < tolerance)
        {
            transform.position = currentTarget;
            delayStart = Time.time;
        }
    }

    private void UpdateTarget()
    {
        if(automatic)
        {
            if(Time.time - delayStart > delayTime)
            {
                NextPlatform();
            }
        }
    }

    public void NextPlatform()
    {
        pointNumber++;
        if(pointNumber >= points.Length)
        {
            pointNumber = 0;
        }
        currentTarget = points[pointNumber];
    }

    private void OnTriggerEnter(Collider other)
    {
        ogScale = other.transform.localScale;

        if (other.gameObject.tag == "Player")
        {
            other.transform.parent = transform;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.parent = null;
        }

    }

}
