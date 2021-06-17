using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExplodingItem : MonoBehaviour
{
    public float radius = 5.0F;
    public float power = 10.0F;

    public AudioClip hit;
    public AudioMixerGroup mixerGroup;
    AudioSource hitSource;

    // Start is called before the first frame update
    void Start()
    {
        hitSource = GetComponent<AudioSource>();
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 2.0F);
        }

        hitSource.Play();

        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {

    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);

    }
}
