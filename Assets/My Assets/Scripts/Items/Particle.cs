using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{

    ParticleSystem self;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        self = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(self.isPlaying)
        {

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
