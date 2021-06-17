using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerMaterials : MonoBehaviour
{
    [SerializeField]
    Material glow;
    [SerializeField]
    Material regular;

    [SerializeField]
    Light playerLight;

    Color lerpedColor;

    bool glowing;
    public bool shieldPowered;
    bool triggered;

    SkinnedMeshRenderer dragon;

    public AudioClip shield;
    public AudioMixerGroup mixerGroup;
    AudioSource shieldSource;

    // Start is called before the first frame update
    void Start()
    {
        dragon = GetComponent<SkinnedMeshRenderer>();
        //regular = dragon.materials[0];

        glow.EnableKeyword("_EMISSION");


        //Debug.Log(dragon.materials[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (!shieldSource)
        {
            shieldSource = gameObject.AddComponent<AudioSource>();
            shieldSource.outputAudioMixerGroup = mixerGroup;
            shieldSource.clip = shield;
            shieldSource.loop = false;
        }

        if (glowing)
        {
            Debug.Log("yes");
            Color orange = new Color32(200, 79, 39, 1);
            Color green1 = new Color32(10, 161, 57, 1);
            Color yellow1 = new Color32(249, 194, 44, 1);

            lerpedColor = Color.Lerp(Color.black, yellow1, Mathf.PingPong(Time.time, 1));
            dragon.material.SetColor("_EmissionColor", lerpedColor * 0.5f);

            playerLight.intensity = Mathf.Lerp(0.0f, 2.0f, Mathf.PingPong(Time.time, 1));
        }

        if(shieldPowered == true && !triggered)
        {
            shieldSource.Play();
            dragon.material = glow;
            glowing = true;
            GameManager.vulnerable = false;
            triggered = true;
            StartCoroutine(Wait());
        }

        //Debug.Log(shieldPowered);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5.0f);
        glowing = false;
        shieldPowered = false;
        triggered = false;
        dragon.material = regular;
        playerLight.intensity = 0.0f;
        GameManager.vulnerable = true;

    }
}
