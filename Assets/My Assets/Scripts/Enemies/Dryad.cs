using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dryad : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;

    public bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        name = "Dryad";
    }

    // Update is called once per frame
    void Update()
    {



        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("Speed", transform.InverseTransformDirection(rb.velocity).z);

    }
}
