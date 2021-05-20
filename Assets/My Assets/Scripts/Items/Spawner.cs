using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spawner : MonoBehaviour
{

    [SerializeField]
    private GameObject[] prefabs;
    [SerializeField]
    private int range;

    private int randomPrefab;
    // Start is called before the first frame update
    void Start()
    {
        //throw exception for out of range
        try
        {
            randomPrefab = UnityEngine.Random.Range(0, range);
            Instantiate(prefabs[randomPrefab], transform.position, Quaternion.identity);
            //Destroy(gameObject);
        }
        catch (IndexOutOfRangeException ex)
        {
            Debug.Log(ex.Message);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
