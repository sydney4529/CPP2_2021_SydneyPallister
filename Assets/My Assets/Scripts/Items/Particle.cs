using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{

    ParticleSystem self;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        self = GetComponent<ParticleSystem>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(self.isPlaying || source.isPlaying)
        //{

        //}
        //else
        //{
        //    Destroy(gameObject);
        //}

        if(!self.isPlaying)
        {
            if(source != null)
            {
                if(!source.isPlaying)
                    Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
