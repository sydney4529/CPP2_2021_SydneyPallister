using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MuteManager : MonoBehaviour
{
    private bool isMuted;

    public AudioMixer mixer;

    public Slider volSlider;

    // Start is called before the first frame update
    void Start()
    {
        isMuted = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MutedPressed()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            mixer.SetFloat("MusicVol", -80);
        }
        else
        {
            mixer.SetFloat("MusicVol", Mathf.Log10(volSlider.value) * 20);
        }
    }
}
